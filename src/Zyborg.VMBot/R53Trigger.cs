using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.EC2.Model;
using Amazon.Route53;
using Amazon.Route53.Model;
using Microsoft.Extensions.Logging;
using Zyborg.VMBot.Util;

namespace Zyborg.VMBot
{
    public class R53Trigger
    {
        public const string R53TriggerTagName = "vmbot:r53";
        public const string R53RouterTriggerTagName = "vmbot:r53-routing";
        public const string R53HealthCheckTriggerTagName = "vmbot:r53-health";

        public const string ContinentPrefix = "CONTINENT=";
        public const string CountryPrefix = "COUNTRY=";

        private ILogger _logger;
        private IAmazonRoute53 _r53;
        private EC2Evaluator _ec2Eval;

        public R53Trigger(ILogger<R53Trigger> logger, IAmazonRoute53 r53, EC2Evaluator ec2Eval)
        {
            _logger = logger;
            _r53 = r53;
            _ec2Eval = ec2Eval;
        }

        public bool HasTrigger(Instance inst, Dictionary<string, string> tags)
        {
            return tags.ContainsKey(R53TriggerTagName);
        }


        /// <summary>
        /// Format of the R53 spec is:
        /// <code>
        ///     &lt;zone-id&gt; ';' &lt;record-name&gt; [ ';' [ &lt;record-type&gt; ] [ ';' [ &lt;record-ttl&gt; ] [ ';' &lt;record-value&gt; ] ] ]
        /// </code>
        /// </summary>
        R53Spec ResolveR53Spec(Instance inst, Dictionary<string, string> tags)
        {
            var specTag = _ec2Eval.Evaluate(tags[R53TriggerTagName], inst);
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
        public R53RoutingSpec ResolveR53RoutingSpec(Instance inst, Dictionary<string, string> tags)
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

        public class R53HealthSpec
        {
            
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

        public async Task HandleInitR53(Instance inst, Dictionary<string, string> tags)
        {
            _logger.LogInformation("Handling CREATING R53 records");

            var r53Spec = ResolveR53Spec(inst, tags);
            var r53Routing = ResolveR53RoutingSpec(inst, tags);

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
                    }
                }
            };

            var changeResp = await _r53.ChangeResourceRecordSetsAsync(changeRequ);
            _logger.LogInformation("UPSERT request completed, response:");
            _logger.LogInformation(JsonSerializer.Serialize(changeResp));
        }

        public async Task HandleTermR53(Instance inst, Dictionary<string, string> tags)
        {
            _logger.LogInformation("Handling REMOVING R53 records");
         
            var r53Spec = ResolveR53Spec(inst, tags);
            var r53Routing = ResolveR53RoutingSpec(inst, tags);

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
            // Optional routing policy configuration
            r53Routing?.Apply(rrset);

            var listRequ = new ListResourceRecordSetsRequest
            {
                HostedZoneId = r53Spec.Zone,
                StartRecordName = r53Spec.Name,
                StartRecordType = r53Spec.Type,
                StartRecordIdentifier = null,
            };

            var listResp = await _r53.ListResourceRecordSetsAsync(listRequ);
            var rr = listResp.ResourceRecordSets.FirstOrDefault();
            if (rr == null
                || rr.Name != r53Spec.Name
                || rr.Type != r53Spec.Type)
            {
                _logger.LogWarning("No existing resource records found; SKIPPING");
                _logger.LogInformation("First returned record for query:");
                _logger.LogInformation(JsonSerializer.Serialize(rr));
                return;
            }

            var changeRequ = new ChangeResourceRecordSetsRequest
            {
                HostedZoneId = r53Spec.Zone,
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
}