using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.EC2.Model;
using Amazon.Route53;
using Amazon.Route53.Model;
using Amazon.S3;
using Microsoft.Extensions.Logging;
using Zyborg.VMBot.CWEvents.EC2StateChange;
using Zyborg.VMBot.Util;

namespace Zyborg.VMBot
{
    public class R53Trigger
    {
        public const string R53RecordTriggerTagName = "vmbot:r53";
        public const string R53RouterTriggerTagName = "vmbot:r53-routing";
        public const string R53HealthCheckTriggerTagName = "vmbot:r53-health";

        public const string ContinentPrefix = "CONTINENT=";
        public const string CountryPrefix = "COUNTRY=";

        public const string HealthCheckS3RefPrefix = "!S3=";

        public static readonly IReadOnlyCollection<HealthCheckType> HealthCheckInlineAllowedTypes =
            new List<HealthCheckType>
            {
                HealthCheckType.HTTP, HealthCheckType.HTTP_STR_MATCH,
                HealthCheckType.HTTPS, HealthCheckType.HTTPS_STR_MATCH,
                HealthCheckType.TCP,
            };

        static readonly JsonSerializerOptions ConstantClassJsonOpts;

        private ILogger _logger;
        private IAmazonRoute53 _r53;
        private IAmazonS3 _s3;
        private EC2Evaluator _ec2Eval;

        static R53Trigger()
        {
            ConstantClassJsonOpts = new JsonSerializerOptions();
            ConstantClassJsonOpts.Converters.Add(new ConstantClassConverter());
        }

        public R53Trigger(ILogger<R53Trigger> logger, IAmazonRoute53 r53, IAmazonS3 s3,
            EC2Evaluator ec2Eval)
        {
            _logger = logger;
            _r53 = r53;
            _s3 = s3;
            _ec2Eval = ec2Eval;
        }

        public bool HasTrigger(Instance inst, Dictionary<string, string> tags)
        {
            return tags.ContainsKey(R53RecordTriggerTagName)
                || tags.ContainsKey(R53HealthCheckTriggerTagName);
        }

        static readonly IReadOnlyCollection<string> ResettableHealthCheckElements = new List<string>
        {
            nameof(UpdateHealthCheckRequest.ChildHealthChecks),
            nameof(UpdateHealthCheckRequest.FullyQualifiedDomainName),
            nameof(UpdateHealthCheckRequest.Regions),
            nameof(UpdateHealthCheckRequest.ResourcePath),
        };

        public async Task HandleInitR53(Instance inst, Dictionary<string, string> tags, string ec2State)
        {
            _logger.LogInformation("Handling CREATING R53 records");

            var r53Spec = ResolveR53RecordSpec(inst, tags);
            var r53Routing = ResolveR53RoutingSpec(inst, tags);
            var r53Health = await ResolveR53HealthCheckSpec(inst, tags);

            HealthCheck existingHealth = await FindExistingHealthCheck(r53Health);

            // We apply the Health Check if any, first because
            // the subsequent Route policy may depend on it
            string healthCheckId = null;
            if (r53Health != null)
            {
                if (existingHealth == null)
                {
                    var createRequ = new CreateHealthCheckRequest
                    {
                        CallerReference = r53Health.RefName,
                        HealthCheckConfig = r53Health.Config,
                    };
                    var createResp = await _r53.CreateHealthCheckAsync(createRequ);
                    _logger.LogInformation("CREATE Health Check request completed, response:");
                    _logger.LogInformation(JsonSerializer.Serialize(createResp));
                    if (createResp.HealthCheck == null)
                        throw new Exception("failed to create Health Check");
                    healthCheckId = createResp.HealthCheck.Id;

                    var tagRequ = new ChangeTagsForResourceRequest
                    {
                        ResourceType = TagResourceType.Healthcheck,
                        ResourceId = healthCheckId,
                        AddTags = new List<Amazon.Route53.Model.Tag>
                        {
                            new Amazon.Route53.Model.Tag
                            {
                                Key = "Name",
                                Value = r53Health.RefName,
                            },
                            new Amazon.Route53.Model.Tag
                            {
                                Key = "vmbot:memo",
                                Value = $"Managed by VMBot {nameof(R53Trigger)}",
                            },
                        },
                    };
                    var tagResp = await _r53.ChangeTagsForResourceAsync(tagRequ);
                    _logger.LogInformation("CHANGED TAGS for Health Check:");
                    _logger.LogInformation(JsonSerializer.Serialize(tagResp));
                }
                else
                {
                    var updateRequ = new UpdateHealthCheckRequest
                    {
                        HealthCheckId = existingHealth.Id,
                        HealthCheckVersion = existingHealth.HealthCheckVersion,
                    };
                    CopyOrReset(existingHealth.HealthCheckConfig, r53Health.Config, updateRequ);
                    _logger.LogInformation("Resolved Health Check delta:");
                    _logger.LogInformation(JsonSerializer.Serialize(updateRequ));
                    var updateResp = await _r53.UpdateHealthCheckAsync(updateRequ);
                    _logger.LogInformation("UPDATE Health Check request completed, response:");
                    _logger.LogInformation(JsonSerializer.Serialize(updateResp));
                    healthCheckId = updateResp.HealthCheck.Id;
                }
            }

            if (r53Spec != null)
            {
                var rrset = new ResourceRecordSet
                {
                    Name = r53Spec.Name,
                    Type = RRType.FindValue(r53Spec.Type),
                    TTL = r53Spec.TTL,
                    ResourceRecords = new List<ResourceRecord>
                    {
                        new ResourceRecord(r53Spec.Value),
                    },
                };
                if (healthCheckId != null)
                    rrset.HealthCheckId = healthCheckId;

                // Optional routing policy configuration
                r53Routing?.Apply(rrset);

                var changeRequ = new ChangeResourceRecordSetsRequest
                {
                    HostedZoneId = r53Spec.Zone,
                    ChangeBatch = new ChangeBatch
                    {
                        Changes = new List<Change>
                        {
                            new Change
                            {
                                Action = ChangeAction.UPSERT,
                                ResourceRecordSet = rrset,
                            }
                        },
                        Comment = "change request applied by VMBot",
                    }
                };

                var changeResp = await _r53.ChangeResourceRecordSetsAsync(changeRequ);
                _logger.LogInformation("UPSERT Resource Record request completed, response:");
                _logger.LogInformation(JsonSerializer.Serialize(changeResp));
            }
        }

        public async Task HandleTermR53(Instance inst, Dictionary<string, string> tags, string ec2State)
        {
            _logger.LogInformation("Handling REMOVING R53 records");
         
            var r53Record = ResolveR53RecordSpec(inst, tags);
            var r53Routing = ResolveR53RoutingSpec(inst, tags);
            var r53Health = await ResolveR53HealthCheckSpec(inst, tags);

            HealthCheck existingHealth = await FindExistingHealthCheck(r53Health);

            if (r53Record != null)
            {
                var rrset = new ResourceRecordSet
                {
                    Name = r53Record.Name,
                    Type = RRType.FindValue(r53Record.Type),
                    TTL = r53Record.TTL,
                    ResourceRecords = new List<ResourceRecord>
                    {
                        new ResourceRecord(r53Record.Value),
                    },
                };
                // Optional routing policy configuration
                r53Routing?.Apply(rrset);

                if (existingHealth != null)
                    rrset.HealthCheckId = existingHealth.Id;

                var listRequ = new ListResourceRecordSetsRequest
                {
                    HostedZoneId = r53Record.Zone,
                    StartRecordName = r53Record.Name,
                    StartRecordType = r53Record.Type,
                    StartRecordIdentifier = r53Routing?.SetIdentifier,
                };

                var listResp = await _r53.ListResourceRecordSetsAsync(listRequ);
                var rr = listResp.ResourceRecordSets.FirstOrDefault();

                if (rr == null
                    || rr.Name != r53Record.Name
                    || rr.Type != r53Record.Type
                    || (r53Routing != null && !string.Equals(rr.SetIdentifier, r53Routing.SetIdentifier)))
                {
                    _logger.LogWarning("No existing resource records found; SKIPPING");
                    _logger.LogInformation("First returned record for query:");
                    _logger.LogInformation(JsonSerializer.Serialize(rr));
                }
                else
                {
                    var changeRequ = new ChangeResourceRecordSetsRequest
                    {
                        HostedZoneId = r53Record.Zone,
                        ChangeBatch = new ChangeBatch
                        {
                            Changes = new List<Change>
                            {
                                new Change
                                {
                                    Action = ChangeAction.DELETE,
                                    ResourceRecordSet = rrset,
                                }
                            }
                        }
                    };

                    var changeResp = await _r53.ChangeResourceRecordSetsAsync(changeRequ);
                    _logger.LogInformation("DELETE request completed, response:");
                    _logger.LogInformation(JsonSerializer.Serialize(changeResp));
                }
            }

            // We delete the Health Check if any, second because
            // the preceding Route policy may depend on it
            if (existingHealth != null &&
                (ec2State == EC2StateChangeStates.ShuttingDown
                || ec2State == EC2StateChangeStates.Terminated))
            {
                if (existingHealth != null)
                {
                    _logger.LogInformation($"Found existing Health Check record [{existingHealth.Id}]");
                    var deleteRequ = new DeleteHealthCheckRequest
                    {
                        HealthCheckId = existingHealth.Id,
                    };
                    var deleteResp = await _r53.DeleteHealthCheckAsync(deleteRequ);
                    _logger.LogInformation("DELETE Health Check request completed, response:");
                    _logger.LogInformation(JsonSerializer.Serialize(deleteResp));
                }
            }
        }

        /// <summary>
        /// Format of the R53 spec is:
        /// <code>
        ///     &lt;zone-id&gt; ';' &lt;record-name&gt; [ ';' [ &lt;record-type&gt; ] [ ';' [ &lt;record-ttl&gt; ] [ ';' &lt;record-value&gt; ] ] ]
        /// </code>
        /// </summary>
        R53Spec ResolveR53RecordSpec(Instance inst, Dictionary<string, string> tags)
        {
            if (!tags.TryGetValue(R53RecordTriggerTagName, out var specTag))
                return null;
            
            specTag = _ec2Eval.Evaluate(specTag, inst);
            var specParts = specTag.Split(";", 5);
            if (specParts.Length < 2)
                throw new Exception("invalid R53 spec");

            if (tags.TryGetValue(R53HealthCheckTriggerTagName, out var healthTag))
            {
                healthTag = _ec2Eval.Evaluate(healthTag, inst);
                _logger.LogInformation($"Adjusting R53 Health Check per resolved spec: {healthTag}");
            }

            _logger.LogInformation($"Adjusting Route53 records per resolved spec: {specTag}");

            var zone = specParts[0];
            var name = specParts[1];
            if (!name.EndsWith("."))
                name += ".";

            var typeRaw = specParts.Length > 2 ? specParts[2] : null;
            var ttlRaw = specParts.Length > 3 ? specParts[3] : null;
            var valRaw = specParts.Length > 4 ? specParts[4] : null;

            // Resolve Type or default to A
            var type = string.IsNullOrEmpty(typeRaw) ? null : RRType.FindValue(typeRaw);
            if (type == null)
            {
                _logger.LogInformation("Defaulting to record type `A`");
                type = RRType.A;
            }

            // Resolve TTL or default to 0
            if (!long.TryParse(ttlRaw, out var ttl))
            {
                _logger.LogInformation("Defaulting to TTL 60s");
                ttl = 60;
            }

            // Resolve the record value
            string val = string.IsNullOrEmpty(valRaw)
                ? inst.PublicIpAddress ?? inst.PrivateIpAddress
                : valRaw;

            return new R53Spec
            {
                Zone = zone,
                Name = name,
                Type = type,
                TTL = ttl,
                Value = val,
            };
        }

        /// <summary>
        /// Format of the R53 Routing spec is:
        /// <code>
        ///     &lt;set-id&gt; ';' &lt;route-type&gt; ';' &lt;route-type-arg&gt;
        /// </code>
        /// 
        /// <c>route-type</c> can be one of the following values, and will determine
        /// how <c>route-type-arg</c> is interpreted:
        /// <list>
        ///     <item>
        ///         <term><c>MULTI</c></term>
        ///         <description>The <c>route-type-arg</c> is optional and ignored.
        ///             You can have up to 8 records configured to use
        ///             the multi-valued record routing policy
        ///             </description>
        ///     </item>
        ///     <item>
        ///         <term><c>WEIGHTED</c></term>
        ///         <description><c>route-type-arg</c> is required and interpreted as
        ///             a relative numerical weight in the range 0-255.
        ///             </description>
        ///     </item>
        ///     <item>
        ///         <term><c>FAILOVER</c></term>
        ///         <description><c>route-type-arg</c> is required and if its value is
        ///             the string <c>PRIMARY</c> (compared case-insensitively) then it is
        ///             configured as a primary failover record, otherwise it will be
        ///             configured as a secondary record.
        ///             </description>
        ///     </item>
        ///     <item>
        ///         <term><c>LATENCY</c></term>
        ///         <description><c>route-type-arg</c> is required and should specify
        ///             the AWS Region from which the latency check will be performed.
        ///             </description>
        ///     </item>
        ///     <item>
        ///         <term><c>GEO</c></term>
        ///         <description><c>route-type-arg</c> is required and should be specified
        ///             in one of two forms:
        ///             <code>
        ///                 'continent=' &lt;continent-code&gt;
        /// 
        ///                 'country=' &lt;country-code&gt; [ ',' &lt;sub-location&gt; ]
        ///             </code>
        ///             </description>
        ///     </item>
        /// </list>
        /// </summary>
        R53RoutingSpec ResolveR53RoutingSpec(Instance inst, Dictionary<string, string> tags)
        {
            if (!tags.TryGetValue(R53RouterTriggerTagName, out var specTag))
                return null;
            
            specTag = _ec2Eval.Evaluate(specTag, inst);
            var specParts = specTag.Split(";", 3);
            if (specParts.Length < 2)
                throw new Exception("invalid R53 Routing Policy spec");

            _logger.LogInformation($"Defining R53 Routing policy per resolved spec: {specTag}");

            var specSetId = specParts[0];
            var specType = specParts[1].ToUpper();
            var spec = new R53RoutingSpec
            {
                SetIdentifier = specSetId,
            };
            
            if (specType == "MULTI")
            {
                spec.MultiValue = true;
            }
            else if (specParts.Length < 3
                || !(specParts[2] is string specArg)
                || string.IsNullOrWhiteSpace(specArg))
            {
                throw new Exception("missing required routing policy argument");
            }
            else if (specType == "WEIGHTED")
            {
                spec.Weight = long.Parse(specArg);
                if (spec.Weight < 0 || spec.Weight > 255)
                    throw new Exception("weight out of range");
            }
            else if (specType == "FAILOVER")
            {
                spec.Failover = specArg.ToUpper() == "PRIMARY"
                    ? ResourceRecordSetFailover.PRIMARY
                    : ResourceRecordSetFailover.SECONDARY;
            }
            else if (specType == "LATENCY")
            {
                spec.LatencyRegion = ResourceRecordSetRegion.FindValue(specArg);
            }
            else if (specType == "GEO")
            {
                var specReg = specArg.ToUpper();
                if (specReg.StartsWith(ContinentPrefix))
                {
                    spec.GeoLocation = new GeoLocation
                    {
                        ContinentCode = specReg.Substring(ContinentPrefix.Length)
                    };
                }
                else if (specReg.StartsWith(CountryPrefix))
                {
                    var codes = specReg.Substring(CountryPrefix.Length).Split(",", 2);
                    spec.GeoLocation = new GeoLocation
                    {
                        CountryCode = codes[0],
                        SubdivisionCode = codes.Length > 1 ? codes[1] : null,
                    };
                }
                else
                {
                    throw new Exception("unknown Geo location type");
                }
            }
            else
            {
                throw new Exception("Unknown routing policy type");
            }

            return spec;
        }

        async Task<R53HealthSpec> ResolveR53HealthCheckSpec(Instance inst,
            Dictionary<string, string> tags)
        {
            if (!tags.TryGetValue(R53HealthCheckTriggerTagName, out var specTag))
                return null;
            
            specTag = _ec2Eval.Evaluate(specTag, inst);

            var hc = new R53HealthSpec
            {
                RefName = $"vmbot-{inst.InstanceId}",
            };

            if (specTag.StartsWith(HealthCheckS3RefPrefix))
            {
                hc.Config = await ParseS3RefHealthCheck(specTag, inst);
            }
            else
            {
                hc.Config = await ParseInlineHealthCheck(specTag);
            }

            _logger.LogInformation("Resolved Health Check Spec as:");
            _logger.LogInformation(JsonSerializer.Serialize(hc));

            return hc;
        }

        /// Inline form:
        ///   <hc-type> ';' <threshold> ';' <ip-addr> ';' <port> ';' <fqdn> ';' <res-path> ';' <search>
        Task<HealthCheckConfig> ParseInlineHealthCheck(string specTag)
        {
            var specParts = specTag.Split(";", 7);
            if (specParts.Length < 2)
                throw new Exception("invalid INLINE R53 Health Check spec");

            _logger.LogInformation($"Defining R53 Health Check per resolved INLINE spec: {specTag}");

            var hcTypeStr = specParts[0];
            var hcType = HealthCheckType.FindValue(hcTypeStr);
            if (hcType == null)
                throw new Exception("unknown Health Check type");
            if (!HealthCheckInlineAllowedTypes.Contains(hcType))
                throw new Exception("Health Check type is unsupported for INLINE spec");

            var config = new HealthCheckConfig
            {
                Type = hcType,
                // RequestInterval = 30,
                // InsufficientDataHealthStatus = InsufficientDataHealthStatus.LastKnownStatus,
            };
            uint numVal;

            if (specParts.Length > 1 && !string.IsNullOrWhiteSpace(specParts[1]))
            {
                if (!uint.TryParse(specParts[1], out numVal))
                    throw new Exception("invalid threshold value");
                config.FailureThreshold = (int)numVal;
            }

            if (specParts.Length > 2 && !string.IsNullOrWhiteSpace(specParts[2]))
            {
                config.IPAddress = specParts[2].Trim();
            }
            
            if (specParts.Length > 3 && !string.IsNullOrWhiteSpace(specParts[3]))
            {
                if (!uint.TryParse(specParts[3], out numVal))
                    throw new Exception("invalid port value");
                config.Port = (int)numVal;
            }
            
            if (specParts.Length > 4 && !string.IsNullOrWhiteSpace(specParts[4]))
            {
                config.FullyQualifiedDomainName = specParts[4].Trim();
            }

            if (specParts.Length > 5 && !string.IsNullOrWhiteSpace(specParts[5]))
            {
                config.ResourcePath = specParts[5];
            }

            if (specParts.Length > 6 && !string.IsNullOrWhiteSpace(specParts[6]))
            {
                config.SearchString = specParts[6];
            }

            return Task.FromResult(config);
        }

        /// S3 Reference form:
        ///   '!S3=' <bucket-name> '/' <key-path>
        async Task<HealthCheckConfig> ParseS3RefHealthCheck(string specTag, Instance inst)
        {   var specParts = specTag.Substring(HealthCheckS3RefPrefix.Length).Split("/", 2);
            if (specParts.Length < 2)
                throw new Exception("invalid S3REF R53 Health Check spec");

            var getResp = await _s3.GetObjectAsync(specParts[0], specParts[1]);
            using var reader = new StreamReader(getResp.ResponseStream);
            var json = _ec2Eval.Evaluate(await reader.ReadToEndAsync(), inst);

            return JsonSerializer.Deserialize<HealthCheckConfig>(json, ConstantClassJsonOpts);
        }

        async Task<HealthCheck> FindExistingHealthCheck(R53HealthSpec r53Health)
        {
            HealthCheck existingHealth = null;
            if (r53Health != null)
            {
                var listRequ = new ListHealthChecksRequest();
                var listResp = await _r53.ListHealthChecksAsync(listRequ);
                _logger.LogInformation("Looking for existing Health Check for RefName: " + r53Health.RefName);
                while (listResp.HealthChecks?.Count > 0)
                {
                    existingHealth = listResp.HealthChecks.FirstOrDefault(hc =>
                        hc.CallerReference == r53Health.RefName);
                    
                    if (existingHealth != null)
                        // Found a match
                        break;
                    
                    if (!listResp.IsTruncated)
                        // No more records
                        break;

                    // Get the next page of results
                    listRequ.Marker = listResp.Marker;
                    listResp = await _r53.ListHealthChecksAsync(listRequ);
                }

                if (existingHealth == null)
                {
                    _logger.LogInformation("Found NO EXISTING Health Check for resolved spec");
                }
                else
                {
                    _logger.LogInformation("Found existing Health Check for resolved spec: " + existingHealth.Id);
                }
            }
            return existingHealth;
        }

        void CopyOrReset(HealthCheckConfig oldConfig, HealthCheckConfig newConfig, UpdateHealthCheckRequest delta)
        {
            var hcType = typeof(HealthCheckConfig);
            var upType = typeof(UpdateHealthCheckRequest);
            var bFlags = BindingFlags.Public | BindingFlags.Instance;

            foreach (var hcProp in hcType.GetProperties(bFlags))
            {
                var oldValue = hcProp.GetValue(oldConfig);
                var newValue = hcProp.GetValue(newConfig);
                // Cheap way to compare values, even for complex objects
                var oldValueJson = JsonSerializer.Serialize(oldValue);
                var newValueJson = JsonSerializer.Serialize(newValue);
                if (oldValueJson != newValueJson)
                {
                    var upProp = upType.GetProperty(hcProp.Name, bFlags);
                    if (upProp == null)
                    {
                        _logger.LogWarning($"Detected Health Check config change for property [{hcProp.Name}]"
                            + " but value CANNOT be updated on existing Health Check");
                    }
                    else
                    {
                        if (newValueJson == null && ResettableHealthCheckElements.Contains(hcProp.Name))
                        {
                            if (delta.ResetElements == null)
                                delta.ResetElements = new List<string>();
                            delta.ResetElements.Add(hcProp.Name);
                        }
                        else
                        {
                            upProp.SetValue(delta, newValue);
                        }
                    }
                }
            }
        }

        // // void foo()
        // // {
        // //     // hc-name = vbot-%ID%
        // //     //
        // //     // Inline form:
        // //     //   <hc-type> ';' <threshold> ';' <ip-addr> ';' <port> ';' <fqdn> ';' <res-path> ';' <search>
        // //     //
        // //     // S3 Reference form:
        // //     //   '!S3=' <bucket-name> '/' <key-path>
        // //     //
        // //     // hc-type:
        // //     //  * HTTP
        // //     //  * HTTP_STR_MATCH
        // //     //  * HTTPS
        // //     //  * HTTPS_STR_MATCH
        // //     //  * TCP

        // //     var TODO = string.Empty;
        // //     var refName = "vmbot-i-abcdef1093942934802";



        // //     var requ = new CreateHealthCheckRequest
        // //     {
        // //         CallerReference = "vbot-%ID%",

        // //         HealthCheckConfig = new HealthCheckConfig
        // //         {
        // //             Type = HealthCheckType.FindValue(TODO),
        // //             FailureThreshold = 3,
        // //             IPAddress = TODO,
        // //             Port = 0,
        // //             FullyQualifiedDomainName = "foo.bar.example.com",
        // //             ResourcePath = TODO,
        // //             SearchString = TODO,



        // //             RequestInterval = 0,

        // //             Inverted = false,
        // //             InsufficientDataHealthStatus = InsufficientDataHealthStatus.FindValue(TODO),
        // //             MeasureLatency = false,
        // //             Regions = new List<string>
        // //             {
        // //                 TODO,
        // //             },

        // //             AlarmIdentifier = new AlarmIdentifier
        // //             {
        // //                 Name = TODO,
        // //                 Region = CloudWatchRegion.FindValue(TODO),
        // //             },
        // //             EnableSNI = false,
        // //             HealthThreshold = 3, // Number of child health checks

        // //             ChildHealthChecks = new List<string> { TODO },
        // //             Disabled = false,
        // //         },
        // //     }.HealthCheckConfig;

        // //     new ListTagsForResourceRequest
        // //     {
        // //         ResourceId = TODO,
        // //         ResourceType = TagResourceType.Healthcheck,
        // //     };

        // //     new ListTagsForResourcesRequest
        // //     {
        // //         ResourceType = TagResourceType.Healthcheck,
        // //     };

        // //     new ListHealthChecksRequest
        // //     {

        // //     };

            

        // //     new UpdateHealthCheckRequest
        // //     {
        // //         HealthCheckId = TODO,
        // //     };

        // //     new DeleteHealthCheckRequest
        // //     {
        // //         HealthCheckId = TODO,
        // //     };
        // // }

        public class R53HealthSpec
        {
            public string RefName { get; set; }
            public HealthCheckConfig Config { get; set; }
        }

        public class R53Spec
        {
            public string Zone { get; set; }
            public string Name { get; set; }
            public RRType Type { get; set; }
            public long TTL { get; set; }
            public string Value { get; set; }
        }

        public class R53RoutingSpec
        {
            public string SetIdentifier { get; set; }
            public bool? MultiValue { get; set; }
            public long? Weight { get; set; }
            public ResourceRecordSetFailover Failover { get; set; }
            public ResourceRecordSetRegion LatencyRegion { get; set; }
            public GeoLocation GeoLocation { get; set; }

            public void Apply(ResourceRecordSet rrset)
            {
                rrset.SetIdentifier = SetIdentifier;
                if (MultiValue.HasValue)
                    rrset.MultiValueAnswer = MultiValue.Value;
                if (Weight.HasValue)
                    rrset.Weight = Weight.Value;
                rrset.Failover = Failover;
                rrset.Region = LatencyRegion;
                rrset.GeoLocation = GeoLocation;
            }
        }
    }
}