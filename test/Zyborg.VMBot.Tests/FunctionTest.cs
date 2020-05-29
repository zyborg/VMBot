using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.S3Events;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

using VMBot;
using Amazon.EC2;
using Amazon.Route53;
using Zyborg.VMBot.CWEvents.EC2StateChange;
using Zyborg.VMBot;

namespace VMBot.Tests
{
    public class FunctionTest
    {
        //[Fact]
        public async Task TestS3EventLambdaFunction()
        {
            var reg = RegionEndpoint.USEast1;
            IAmazonEC2 ec2 = new AmazonEC2Client(reg);
            IAmazonRoute53 r53 = new AmazonRoute53Client(reg);
            IAmazonS3 s3 = new AmazonS3Client(reg);

            var bucketName = "lambda-VMBot-".ToLower() + DateTime.Now.Ticks;
            var key = "text.txt";

            // Create a bucket an object to setup a test data.
            await s3.PutBucketAsync(bucketName);
            try
            {
                await s3.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    ContentBody = "sample data"
                });

                // Setup the S3 event object that S3 notifications would create with the fields used by the Lambda function.
                var s3Event = new S3Event
                {
                    Records = new List<S3EventNotification.S3EventNotificationRecord>
                    {
                        new S3EventNotification.S3EventNotificationRecord
                        {
                            S3 = new S3EventNotification.S3Entity
                            {
                                Bucket = new S3EventNotification.S3BucketEntity {Name = bucketName },
                                Object = new S3EventNotification.S3ObjectEntity {Key = key }
                            }
                        }
                    }
                };

                var ec2StateChange = new EC2StateChangeEvent
                {

                };

                // Invoke the lambda function and confirm the content type was returned.
                var function = new Function(ec2, r53, s3);
                // var contentType = await function.FunctionHandler(s3Event, null);
                var contentType = await function.FunctionHandler(ec2StateChange, null);

                Assert.Equal("text/plain", contentType);

            }
            finally
            {
                // Clean up the test data
                await AmazonS3Util.DeleteS3BucketWithObjectsAsync(s3, bucketName);
            }
        }
    }
}
