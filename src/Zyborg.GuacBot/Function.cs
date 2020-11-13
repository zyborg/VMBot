using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.EC2;
using Amazon.Lambda.CloudWatchEvents;
using Amazon.EC2.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Zyborg.VMBot.Util;
using Zyborg.AWS.Lambda;
using Zyborg.AWS.Lambda.Events;
using Newtonsoft.Json.Linq;
using Zyborg.GuacBot.Model;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Zyborg.GuacBot.GuacAPI;
using Zyborg.GuacBot.GuacAPI.Model;
using Zyborg.GuacBot.GuacDB.Data;
using Zyborg.GuacBot.GuacDB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Amazon.Lambda.SQSEvents;
using System.Threading;
using NServiceBus;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
//[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Zyborg.GuacBot
{
    public class Function : MultiEventFunction<Function, string>
    {
        public const string NameTag = "Name";
        public const string GuacNameTag = "guacbot:name";
        public const string GroupPathTag = "guacbot:path";
        public const string ProtoTag = "guacbot:proto";
        public const string ParamTagPrefix = "guacbot:p:";

        public const string InstanceIdParam = "aws:instance_id";

        // protected IConfiguration _config;
        // protected IServiceProvider _services;
        // protected ILogger _logger;

        private ILogger _logger;

        private static FunctionSettings _settings;

        private IAmazonEC2 _ec2;

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance.
        /// When invoked in a Lambda environment the AWS credentials will come from the IAM role
        /// associated with the function and the AWS region will be set to the region the Lambda
        /// function is executed in.
        /// </summary>
        public Function()
            : this(new AmazonEC2Client())
        { }

        /// <summary>
        /// Constructs an instance with a preconfigured AWS client(s). This can be used for testing
        /// outside of the Lambda environment.
        /// </summary>
        /// <param name="s3"></param>
        public Function(IAmazonEC2 ec2)
        {
            _ec2 = ec2;

            //Setup();
            
            //Initialize(_services);

            Initialize();
        }

        // private void Setup()
        // {
        //     _config = SetupConfiguration();
        //     _services = SetupServices(_config);
        //     _logger = _services.GetRequiredService<ILogger<Function>>();
        //     _settings = _services.GetRequiredService<FunctionSettings>();

        //     var asm = System.Reflection.Assembly.GetExecutingAssembly();
        //     var ver = asm.GetName().Version;

        //     _logger.LogInformation($"*************************************************************");
        //     _logger.LogInformation($"** GuacBot v{ver} - Starting Up...");
        //     _logger.LogInformation($"*************************************************************");
        // }

        // private IConfiguration SetupConfiguration()
        // {
        //     var builder = new ConfigurationBuilder()
        //         .SetBasePath(Directory.GetCurrentDirectory())
        //         .AddJsonFile("appsettings.json", optional: true)
        //         .AddEnvironmentVariables("GUACBOT_");
        //     return builder.Build();
        // }

        // public override void RegisterEventTypeMatchers()
        // {
        //     base.RegisterEventTypeMatchers();
        //     RegisterCommonEventMatchers();
        //     Register<CloudWatchEvent<EC2StateChangeDetail>>(EC2StateChangeDetail.Matcher);
        //     Register<RebuildInstancesCustomEvent>(RebuildInstancesCustomEvent.Matcher);
        // }

        protected override void PrepareAppConfiguration(IConfigurationBuilder builder)
        {
            // We intentionally DO NOT want to call the base implementation
            // so that we can override some of the default config behavior
            //base.ConfigureAppConfiguration(builder);

            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables("GUACBOT_");
        }

        protected override void PrepareServices(IServiceCollection services, IConfiguration config)
        {
            base.PrepareServices(services, config);

            _settings = config.Get<FunctionSettings>();
            if (_settings == null)
            {
                throw new Exception("failed to resolve settings");
            }
            else
            {
                Console.WriteLine("Resolved settings from Configuration:");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(_settings));
            }

            services.AddSingleton<IAmazonEC2>(_ec2);
            services.AddSingleton<EC2Evaluator>();
            services.AddSingleton(_settings);

            services.AddGuacDBContext(config.GetConnectionString("GuacDB"));
        }

        protected override void PrepareFinal(ILogger<Function> logger, IConfiguration config, IServiceProvider services)
        {
            base.PrepareFinal(logger, config, services);

            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var ver = asm.GetName().Version;

            _logger = logger;
            _logger.LogInformation($"*************************************************************");
            _logger.LogInformation($"** GuacBot v{ver} - Starting Up...");
            _logger.LogInformation($"*************************************************************");

            RegisterCommonEventMatchers();
            Register<CloudWatchEvent<EC2StateChangeDetail>>(EC2StateChangeDetail.Matcher);
            Register<RebuildInstancesCustomEvent>(RebuildInstancesCustomEvent.Matcher);
        }


        [LambdaEventHandler]
        public Task<string> DefaultFunctionHandler(JToken jtoken, ILambdaContext context)
        {
            var result = "DEFAULT Handler Invoked -- don't know what to do with: " + jtoken;
            _logger.LogInformation("Returning DEFAULT result:");
            _logger.LogInformation(result);
            return Task.FromResult(result);
        }

        /// <summary>
        /// This method is called for every Lambda invocation.
        /// This method responds to EC2 state change events to apply
        /// various operations based on the target EC2 state and the
        /// various tags applied.
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [LambdaEventHandler(typeof(CloudWatchEvent<EC2StateChangeDetail>))]
        public Task<string> FunctionHandler(CloudWatchEvent<EC2StateChangeDetail> ev, ILambdaContext context)
        {
            _logger.LogInformation("FunctionHandler running...");
            _logger.LogInformation("Got EC2 state change:");
            _logger.LogInformation(System.Text.Json.JsonSerializer.Serialize(ev));

            using (_logger.BeginScope(context.AwsRequestId))
            {
                switch (ev.Detail.State)
                {
                    case EC2StateChangeStates.Pending:
                    case EC2StateChangeStates.Running:
                        //await HandleInstInit(ev.Detail.InstanceId, ev.Detail.State);
                        break;

                    case EC2StateChangeStates.Stopping:
                    case EC2StateChangeStates.Stopped:
                    case EC2StateChangeStates.ShuttingDown:
                    case EC2StateChangeStates.Terminated:
                        //await HandleInstTerm(ev.Detail.InstanceId, ev.Detail.State);
                        break;
                }
            }

            var result = "This is the EC2 STATE CHANGE handler: " + ev.Detail.State;
            return Task.FromResult(result);
        }

        private static readonly NServiceBus.AwsLambdaSQSEndpoint _lambdaSqsEndpoint =
            new NServiceBus.AwsLambdaSQSEndpoint(context =>
            {
                var endpointConfig = new NServiceBus.AwsLambdaSQSEndpointConfiguration(
                    "GuacBot");
                endpointConfig.AdvancedConfiguration.SendFailedMessagesTo("GuacBot-Errors");
                endpointConfig.UseSerialization<NewtonsoftSerializer>();

                if (!string.IsNullOrEmpty(_settings.NsbLicenseBody))
                {
                    context.Logger.LogLine("Setting NSB License from content ENV");
                    endpointConfig.AdvancedConfiguration.License(_settings.NsbLicenseBody);
                }
                else if (!string.IsNullOrEmpty(_settings.NsbLicensePath))
                {
                    var licUrl = new Uri(_settings.NsbLicensePath);
                    if (licUrl.Scheme == "s3")
                    {
                        throw new NotImplementedException("S3 URL Path not implemented yet!");
                    }

                    using (var wc = new System.Net.WebClient())
                    {
                        endpointConfig.AdvancedConfiguration.License(wc.DownloadString(licUrl));
                    }
                }

                return endpointConfig;
            });

        [LambdaEventHandler(typeof(SQSEvent))]
        public async Task<string> FunctionHandler(SQSEvent ev, ILambdaContext context)
        {
            var cancelDelay = context.RemainingTime.Subtract(TimeSpan.FromSeconds(5));
            using (var cancelTokenSource = new CancellationTokenSource(cancelDelay))
            {
                _logger.LogInformation("NSB Processing incoming SQS event");
                await _lambdaSqsEndpoint.Process(ev, context, cancelTokenSource.Token);
            }
            return "Handled SQS Event";
        }

        [LambdaEventHandler(typeof(RebuildInstancesCustomEvent))]
        public Task<string> FunctionHandler(RebuildInstancesCustomEvent rebuild, ILambdaContext context)
        {
            return RebuildInstances(rebuild, context);
        }

        internal async Task<Instance> GetInstance(string id)
        {
            var instRequ = new DescribeInstancesRequest
            {
                InstanceIds = new List<string> { id },
            };
            var instResp = await _ec2.DescribeInstancesAsync(instRequ);

            if (instResp?.Reservations?.Count < 1
                || instResp.Reservations[0].Instances?.Count < 1)
                throw new Exception("no Instance found for given ID");

            _logger.LogInformation($"Resolved Instance for ID {id}");
            return instResp.Reservations[0].Instances[0];
        }

        internal async Task<string> RebuildInstances(
            RebuildInstancesCustomEvent ev, ILambdaContext context)
        {
            var included = new List<string>();

            if (ev.IncludeNameRegex?.Length > 0 || ev.ExcludeNameRegex ?.Length > 0)
            {
                _logger.LogInformation("Searching by Name tag");
                var tagsRequ = new DescribeTagsRequest(new List<Filter>
                {
                    new Filter("resource-type", new List<string> { "instance" }),
                    new Filter("tag:Name", new List<string> { "*" }),
                });

                if (ev.IncludeIds?.Length > 0)
                {
                    _logger.LogInformation("restricting to instance IDs");
                    tagsRequ.Filters.Add(new Filter("resource-id", ev.IncludeIds.ToList()));
                }

                var tagsResp = await _ec2.DescribeTagsAsync(tagsRequ);

                if (tagsResp.Tags.Count == 0)
                {
                    _logger.LogInformation("No matching tags found, SKIPPING");
                    return "NO-MATCH";
                }

                if (ev.ExcludeIds?.Length > 0)
                {
                    _logger.LogInformation("excluding IDs");
                    tagsResp.Tags.RemoveAll(t => ev.ExcludeIds.Contains(t.ResourceId));
                    if (tagsResp.Tags.Count == 0)
                    {
                        _logger.LogInformation("All matching tags EXCLUDED by ID, SKIPPING");
                        return "EXCLUDED-BY-IncludeIds";
                    }
                }

                if (ev.IncludeNameRegex?.Length > 0)
                {
                    _logger.LogInformation("including matched Name tag");
                    foreach (var pattern in ev.IncludeNameRegex)
                    {
                        tagsResp.Tags.RemoveAll(t => !Regex.IsMatch(t.Value, pattern));
                    }
                    if (tagsResp.Tags.Count == 0)
                    {
                        _logger.LogInformation("All matching tags EXCLUDED by Name includes, SKIPPING");
                        return "EXCLUDED-BY-IncludeNameRegex";
                    }
                }

                if (ev.ExcludeNameRegex?.Length > 0)
                {
                    _logger.LogInformation("excluding matched Name tag");
                    foreach (var pattern in ev.ExcludeNameRegex)
                    {
                        // var matches tagsRequ.Tags.Where(t => Regex.IsMatch(t.Value, pattern));
                        // _logger.LogInformation("Removing ")

                        tagsResp.Tags.RemoveAll(t => Regex.IsMatch(t.Value, pattern));
                    }
                    if (tagsResp.Tags.Count == 0)
                    {
                        context.Logger.LogLine("All matching tags EXCLUDED by Name excludes, SKIPPING");
                        return "EXCLUDED-BY-ExcludeNameRegex";
                    }
                }

                included.AddRange(tagsResp.Tags.Select(t => t.ResourceId));
                _logger.LogInformation($"found [{included.Count}] Instances IDs based on Name tag matches");                
            }
            else if (ev.IncludeIds?.Length > 0)
            {
                included.AddRange(ev.IncludeIds);
                _logger.LogInformation($"limiting to [{included.Count}] provided Instances IDs");                
            }
            else
            {
                var allInstances = await _ec2.DescribeInstancesAsync();
                included.AddRange(allInstances.Reservations.SelectMany(
                    r => r.Instances.Select(i => i.InstanceId)));
                _logger.LogInformation($"found ALL [{included.Count}] Instances IDs");                
            }

            if (ev.ExcludeIds?.Length > 0)
            {
                _logger.LogInformation($"excluding by provided Instances IDs");
                foreach (var id in ev.ExcludeIds)
                {
                    included.RemoveAll(i => i == id);
                }
                if (included.Count == 0)
                {
                    _logger.LogInformation("All resolved Instance IDs removed by excluded IDs");
                    return "EXCLUDED-BY-ExcludeIds";
                }
            }

            _logger.LogInformation($"resolved to [{included.Count}] instance ID(s)");


            _logger.LogInformation("spawning tasks to handle each instance...");
            var responses = included.ToDictionary(i => i, i => HandleChangeEvent(new ChangeDetails
            {
                InstanceId = i,
                Ec2StateChange = ev.State,
            }, context));
            _logger.LogInformation("...waiting for all tasks to return...");
            Task.WaitAll(responses.Select(r => r.Value).ToArray());
            _logger.LogInformation("...finished.");
            return System.Text.Json.JsonSerializer.Serialize(responses.ToDictionary(
                r => r.Key, r => r.Value.Result));
        }


        internal async Task<string> HandleChangeEvent(ChangeDetails details,
            ILambdaContext context)
        {

            if (!await ResolveStageChange(details, context))
                return "N/A";

            if (!await ResolveInstanceDetails(details, context))
                return "N/A";

            if (!await ApplyDbChanges(details, context))
                return "NOOP";

            return "DONE!";

        }

        internal Task<bool> ResolveStageChange(ChangeDetails details, ILambdaContext context)
        {
            if (string.IsNullOrEmpty(details.InstanceId))
            {
                _logger.LogInformation("missing or empty Instance ID");
                return Task.FromResult(false);
            }

            _logger.LogInformation($"Got EC2 state change for [{details.InstanceId}]");
            if (details.Ec2StateChange == EC2StateChangeStates.Pending)
            {
                _logger.LogInformation("  Got EC2 PENDING state change");
            }
            else if (details.Ec2StateChange == EC2StateChangeStates.Stopped
                || details.Ec2StateChange == EC2StateChangeStates.Terminated)
            {
                _logger.LogInformation("  Got EC2 STOPPED/TERMINATED state change");
                details.IsDelete = true;
            }
            else
            {
                _logger.LogInformation("Unsupported state change: " + details.Ec2StateChange);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        internal async Task<bool> ResolveInstanceDetails(ChangeDetails details, ILambdaContext context)
        {
            var ec2Resp = await _ec2.DescribeInstancesAsync(new DescribeInstancesRequest
            {
                InstanceIds = new List<string> { details.InstanceId },
            });

            _logger.LogInformation("Got EC2 details:");
            details.Instance = ec2Resp.Reservations?.FirstOrDefault()?.Instances?.FirstOrDefault();
            if (details.Instance == null)
            {
                _logger.LogInformation("Invalid ID");
                return false;
            }
            else
            {
                _logger.LogInformation($"InstanceId={details.Instance.InstanceId}");
            }

            // For debugging:
            //_logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(details.Instance));

            return true;
        }

        internal async Task<bool> ApplyDbChanges(ChangeDetails details, ILambdaContext context)
        {
            return false;

            // details.NameTag = details.GetTag(GuacNameTag) ?? details.GetTag(NameTag);
            // details.ConnectionProto = details.GetTag(ProtoTag);
            // details.ConnectionName = ExpandEmbeddedTokens(details, details.NameTag)
            //     ?? details.Instance.PrivateIpAddress
            //     ?? details.Instance.InstanceId;


            // if (string.IsNullOrEmpty(details.ConnectionProto))
            // {
            //     _logger.LogInformation($"EC2 [{details.NameTag}] is not marked for Guac");
            //     return false;
            // }

            // _logger.LogInformation($"Managing Guac Connection Profile for [{details.NameTag}][{details.InstanceId}]");
            // using (var db = _services.GetRequiredService<GuacDBContext>())
            // // // using (var dbConnProf = new GuacDBContext(_dbOptions))
            // {
            //     GuacamoleConnection conn = null;
            //     bool newConnection = false;
            //     var instIdParam = await db.GuacamoleConnectionParameters
            //         .FirstOrDefaultAsync(p => p.ParameterName == InstanceIdParam
            //             && p.ParameterValue == details.InstanceId);
                
            //     if (instIdParam != null)
            //     {
            //         // Bring in the connection and all related child entities
            //         conn = await db.GuacamoleConnections
            //             .Include(c => c.GuacamoleConnectionParameters)
            //             .Include(c => c.GuacamoleConnectionPermissions)
            //             .Include(c => c.PrimaryGuacamoleSharingProfiles)
            //             .FirstAsync(c => c.ConnectionId == instIdParam.ConnectionId);
            //         _logger.LogInformation("Existing Connection Profile already exists!"
            //             + $" [{conn.ConnectionId}][{conn.ConnectionName??"N/A"}][{JsonSerializer.Serialize(instIdParam)}]");
            //     }

            //     if (details.IsDelete)
            //     {
            //         if (conn != null)
            //         {
            //             _logger.LogInformation($"***** Removing EXISTING Connection Profile [{conn.ConnectionId}]");

            //             db.RemoveRange(conn.GuacamoleConnectionAttributes);
            //             db.RemoveRange(conn.GuacamoleConnectionHistories);
            //             db.RemoveRange(conn.GuacamoleConnectionParameters);
            //             db.RemoveRange(conn.GuacamoleConnectionPermissions);
            //             db.RemoveRange(conn.PrimaryGuacamoleSharingProfiles);
            //             db.Remove(conn);
            //         }
            //         else
            //         {
            //             // NOOP
            //             _logger.LogInformation("***** WARN: found no existing Connection Profile to delete; skipping");
            //             return false;
            //         }
            //     }
            //     else
            //     {
            //         if (conn == null)
            //         {
            //             _logger.LogInformation("***** Adding NEW Connection Profile");

            //             newConnection = true;
            //             conn = new GuacamoleConnection();
            //             conn.GuacamoleConnectionParameters.Add(new GuacamoleConnectionParameter
            //             {
            //                 ParameterName = InstanceIdParam,
            //                 ParameterValue = details.InstanceId,
            //             });
            //             db.Add(conn);
            //         }
            //         else
            //         {
            //             _logger.LogInformation($"***** Updating EXISTING Connection Profile [{conn.ConnectionId}]");
            //             db.Update(conn);
            //         }
            //         conn.ConnectionName = ExpandEmbeddedTokens(details, details.ConnectionName);
            //         conn.Protocol = details.ConnectionProto;

            //         // Resolve Connection Group
            //         var groupPath = details.GetTag(GroupPathTag);
            //         if (!string.IsNullOrEmpty(groupPath))
            //         {
            //             // Because we may have to create the connection groups to match the
            //             // requested path, and we want to prevent duplicates in case there
            //             // are multiple instances of this function running hitting the same
            //             // path segments, we lock the table, resolve the groups (finding
            //             // existing ones or creating new ones) and then commit any changes
            //             // as quickly as possible.  To that end, we setup a new DB context
            //             // to support this action to limit the lock time and outside impact.

            //             _logger.LogInformation($"group path specified, resolving [{groupPath}]");

            //             var parent = (GuacamoleConnectionGroup)null;


            //             using (var dbGroups = _services.GetRequiredService<GuacDBContext>())
            //             using (var dbTx = dbGroups.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
            //             {
            //                 var tmeta = dbGroups.Model.FindRuntimeEntityType(typeof(GuacamoleConnectionGroup));
            //                 var tname = $"{tmeta.GetTableName()}";
            //                 try
            //                 {
            //                     if (_lockGroupTable)
            //                     {
            //                         _logger.LogInformation($"locking group path table [{tname}]");

            //                         // IMPL NOTE:
            //                         // THIS IS "OH SO KLUDGEY"!!!
            //                         // This locking mechanism was cobbled together as a quick and dirty
            //                         // hack to use MySQL's table locking mechanism in concert with
            //                         // transactions to implement a guard around the connection group
            //                         // table when multiple Lambda instances are running.  It does seem
            //                         // to work based on experimentation but it's hard-coded to the
            //                         // schema and a MySQL-only solution and possibly very fragile and
            //                         // easily broken.  TODO:  FIND A BETTER SOLUTION
            //                         // One possibility is to define a separate "lock table" and use a
            //                         // single row in the table to lock access across processes.

            //                         await dbGroups.Database.OpenConnectionAsync();
            //                         var adoCon = dbGroups.Database.GetDbConnection();
            //                         var adoCmd = adoCon.CreateCommand();
            //                         adoCmd.CommandType = System.Data.CommandType.Text;
            //                         //adoCmd.CommandText = $"LOCK TABLES {tname} AS g WRITE";
            //                         adoCmd.CommandText = $"LOCK TABLES guacamole_connection_group WRITE, guacamole_connection_group AS g WRITE";
            //                         await adoCmd.ExecuteNonQueryAsync();

            //                         // await dbGroups.Database.ExecuteSqlCommandAsync($"LOCK TABLES guacamole_connection_group AS g WRITE");
            //                     }
                                
            //                     var groups = dbGroups.GuacamoleConnectionGroup.ToArray();
            //                     var segs = groupPath.Split("/");
            //                     foreach (var s in segs)
            //                     {
            //                         var g = groups.FirstOrDefault(x => x.Parent == parent && x.ConnectionGroupName == s);
            //                         if (g == null)
            //                         {
            //                             _logger.LogInformation($"adding new group for [{s}]");
            //                             g = new GuacamoleConnectionGroup
            //                             {
            //                                 Parent = parent,
            //                                 ConnectionGroupName = s,
            //                             };
            //                             dbGroups.Add(g);
            //                         }
            //                         else
            //                         {
            //                             _logger.LogInformation($"found existing group for [{s}][{g.ConnectionGroupId}]");
            //                         }
            //                         parent = g;
            //                     }
            //                     await dbGroups.SaveChangesAsync();
            //                     dbTx.Commit();
            //                 }
            //                 finally
            //                 {
            //                     if (_lockGroupTable)
            //                     {
            //                         _logger.LogInformation($"releasing table locks");
            //                         await dbGroups.Database.ExecuteSqlCommandAsync($"UNLOCK TABLES");
            //                     }
            //                 }
            //             }

            //             dbConnProf.Attach(parent);
            //             conn.Parent = parent;
            //         }

            //         // Some conflict detection and handling
            //         if (newConnection)
            //         {
            //             var connParent = (conn.Parent?.ConnectionGroupId).GetValueOrDefault(0);
            //             var connName = conn.ConnectionName;
            //             _logger.LogInformation($"testing for possible connection conflicts for [{connParent}][{connName}]...");
            //             var connConflicts = dbConnProf.GuacamoleConnection.Where(x => x.ConnectionName == connName
            //                 && (x.ParentId.GetValueOrDefault(0) == connParent));
            //             if ((connConflicts.ToArray()?.Length??0) > 0)
            //             {
            //                 _logger.LogInformation("WARN: detected conflicts!  adjusting connection name");
            //                 conn.ConnectionName += "DUP-" + DateTime.Now.ToString("HHmmssff");
            //             }
            //             else
            //             {
            //                 _logger.LogInformation("...no conflicts found");
            //             }
            //         }

            //         // Merge connection parameter tags with existing parameter values (if any)
            //         foreach (var paramTag in details.GetTags(startsWith: ParamTagPrefix))
            //         {
            //             // Resolve the name of the parameter
            //             var paramName = paramTag.Key.Substring(ParamTagPrefix.Length);
            //             // Resolve any existing parameter setting
            //             var connParam = conn.GuacamoleConnectionParameter.FirstOrDefault(x => x.ParameterName == paramName);
            //             // If none exists, create it
            //             if (connParam == null)
            //             {
            //                 connParam = new GuacamoleConnectionParameter { ParameterName = paramName, };
            //                 conn.GuacamoleConnectionParameter.Add(connParam);
            //             }
            //             // Set the value (can't be null so default to empty value)
            //             connParam.ParameterValue = ExpandEmbeddedTokens(details, paramTag.Value) ?? "";
            //         }
            //     }

            //     await db.SaveChangesAsync();
            // }

            // return true;
        }

        /*
            Tags:
                "vmbot:r53"        = "domain.sfx;name"
                "vmbot:r53-health" = "domain.sfx;name"
        */

        // public async Task HandleInstInit(string id, string ec2State)
        // {
        //     var inst = await GetInstance(id);
        //     var tags = inst.Tags.ToDictionary(t => t.Key, t => t.Value);

        //     if (_r53Trigger.HasTrigger(inst, tags))
        //         await _r53Trigger.HandleInitR53(inst, tags, ec2State);
        // }

        // public async Task HandleInstTerm(string id, string ec2State)
        // {
        //     var inst = await GetInstance(id);
        //     var tags = inst.Tags.ToDictionary(t => t.Key, t => t.Value);

        //     if (_r53Trigger.HasTrigger(inst, tags))
        //         await _r53Trigger.HandleTermR53(inst, tags, ec2State);
        // }

        // /// <summary>
        // /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        // /// to respond to S3 notifications.
        // /// </summary>
        // /// <param name="ev"></param>
        // /// <param name="context"></param>
        // /// <returns></returns>
        // public async Task<string> FunctionHandler(S3Event ev, ILambdaContext context)
        // {
        //     var s3Event = ev.Records?[0].S3;
        //     if(s3Event == null)
        //     {
        //         return null;
        //     }

        //     try
        //     {
        //         var response = await this._s3.GetObjectMetadataAsync(s3Event.Bucket.Name, s3Event.Object.Key);
        //         return response.Headers.ContentType;
        //     }
        //     catch(Exception e)
        //     {
        //         _logger.LogInformation($"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
        //         _logger.LogInformation(e.Message);
        //         _logger.LogInformation(e.StackTrace);
        //         throw;
        //     }
        // }
    }
}
