using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.EC2;
using Amazon.S3;
using Amazon.S3.Util;
using Amazon.Route53;
using Amazon.Lambda.CloudWatchEvents;
using Zyborg.VMBot.CWEvents.EC2StateChange;
using Amazon.EC2.Model;
using Amazon.Route53.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Zyborg.VMBot.Util;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
//[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Zyborg.VMBot
{
    public class Function
    {
        private IServiceProvider _services;
        private ILogger _logger;

        private IAmazonEC2 _ec2;
        private IAmazonRoute53 _r53;
        private IAmazonS3 _s3;

        private R53Trigger _r53Trigger;

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance.
        /// When invoked in a Lambda environment the AWS credentials will come from the IAM role
        /// associated with the function and the AWS region will be set to the region the Lambda
        /// function is executed in.
        /// </summary>
        public Function() : this(
            new AmazonEC2Client(),
            new AmazonRoute53Client(),
            new AmazonS3Client()
        )
        { }

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client. This can be used for testing
        /// the outside of the Lambda environment.
        /// </summary>
        /// <param name="s3"></param>
        public Function(IAmazonEC2 ec2, IAmazonRoute53 r53, IAmazonS3 s3)
        {
            _ec2 = ec2;
            _r53 = r53;
            _s3 = s3;

            Setup();
        }

        private void Setup()
        {
            var config = SetupConfiguration();
            var services = new ServiceCollection();

            services.AddLogging(logging => SetupLogging(logging, config));
            services.AddSingleton<IAmazonEC2>(_ec2);
            services.AddSingleton<IAmazonRoute53>(_r53);
            services.AddSingleton<IAmazonS3>(_s3);
            services.AddSingleton<EC2Evaluator>();
            services.AddSingleton<R53Trigger>();
            

            _services = services.BuildServiceProvider();
            _logger = _services.GetRequiredService<ILogger<Function>>();

            _r53Trigger = _services.GetRequiredService<R53Trigger>();
        }

        private IConfiguration SetupConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();
            return builder.Build();
        }

        private void SetupLogging(ILoggingBuilder logging, IConfiguration config)
        {
            var opts = new LambdaLoggerOptions(config);
            // var opts = new LambdaLoggerOptions
            // {
            //     IncludeCategory = true,
            //     IncludeEventId = true,
            //     IncludeException = true,
            //     IncludeLogLevel = true,
            //     IncludeNewline = true,
            //     IncludeScopes = true,
            // }

            logging.AddLambdaLogger(opts);
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
        public async Task<string> FunctionHandler(EC2StateChangeEvent ev, ILambdaContext context)
        {
            _logger.LogInformation("Got EC2 state change:");
            _logger.LogInformation(JsonSerializer.Serialize(ev));

            using (_logger.BeginScope(context.AwsRequestId))
            {
                switch (ev.Detail.State)
                {
                    case EC2StateChangeStates.Pending:
                    case EC2StateChangeStates.Running:
                        await HandleInstInit(ev.Detail.InstanceId, ev.Detail.State);
                        break;

                    case EC2StateChangeStates.Stopping:
                    case EC2StateChangeStates.Stopped:
                    case EC2StateChangeStates.ShuttingDown:
                    case EC2StateChangeStates.Terminated:
                        await HandleInstTerm(ev.Detail.InstanceId, ev.Detail.State);
                        break;
                }
            }
            return string.Empty;
        }

        public async Task<Instance> GetInstance(string id)
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

        /*
            Tags:
                "vmbot:r53"        = "domain.sfx;name"
                "vmbot:r53-health" = "domain.sfx;name"
        */

        public async Task HandleInstInit(string id, string ec2State)
        {
            var inst = await GetInstance(id);
            var tags = inst.Tags.ToDictionary(t => t.Key, t => t.Value);

            if (_r53Trigger.HasTrigger(inst, tags))
                await _r53Trigger.HandleInitR53(inst, tags, ec2State);
        }

        public async Task HandleInstTerm(string id, string ec2State)
        {
            var inst = await GetInstance(id);
            var tags = inst.Tags.ToDictionary(t => t.Key, t => t.Value);

            if (_r53Trigger.HasTrigger(inst, tags))
                await _r53Trigger.HandleTermR53(inst, tags, ec2State);
        }

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
        //         context.Logger.LogLine($"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
        //         context.Logger.LogLine(e.Message);
        //         context.Logger.LogLine(e.StackTrace);
        //         throw;
        //     }
        // }
    }
}
