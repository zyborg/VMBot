using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.EC2.Model;
using Zyborg.VMBot.Util;
using Xunit;

namespace VMBot.Tests.Util
{
    public class EC2EvaluatorTests
    {
        public static Instance SampleInstance()
        {
            return new Instance
            {
                InstanceId = "i-abcdefghijk0123456789",
                PublicDnsName = "test.pub-ec2.local",
                PrivateDnsName = "test.prv-ec2.local",
                PublicIpAddress = "1.1.1.1",
                PrivateIpAddress = "10.10.10.10",
                LaunchTime = DateTime.UnixEpoch,
                Tags = new List<Tag>
                {
                    new Tag("tag1", "value1"),
                    new Tag("tag2", "value2"),
                    new Tag("tag3", "value3"),
                }
            };
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData("a", "a")]
        [InlineData("a b c", "a b c")]
        [InlineData("%", "%")]
        [InlineData(" a %", " a %")]
        [InlineData("% c ", "% c ")]
        [InlineData(" a % c ", " a % c ")]

        [InlineData("%foo%", "%foo%")]
        [InlineData(" a %foo%", " a %foo%")]
        [InlineData("%foo% c ", "%foo% c ")]
        [InlineData(" a %foo% c ", " a %foo% c ")]
        [InlineData(" a %foo%% c ", " a %foo%% c ")]

        [InlineData("%PRIVATE_IP%", "10.10.10.10")]
        [InlineData(" a %%%PRIVATE_IP%% c ", " a %10.10.10.10% c ")]
        [InlineData(" a %PRIVATE_IP%", " a 10.10.10.10")]
        [InlineData("%PRIVATE_IP% c ", "10.10.10.10 c ")]
        [InlineData(" %PRIVATE_IP% %PUBLIC_IP% %PUBLIC_DNS% ",
            " 10.10.10.10 1.1.1.1 test.pub-ec2.local ")]
        [InlineData(" %PRIVATE_IP% %TAG:tag1% %PUBLIC_DNS% %TAG:tag2% ",
            " 10.10.10.10 value1 test.pub-ec2.local value2 ")]

        [InlineData("%TAG:NoSuchTag%", "%TAG:NoSuchTag%")]            
        [InlineData("%TAG?:NoSuchTag%", "")]

        [InlineData("%LAUNCH_TIME%", "1/1/1970 12:00:00 AM")]
        [InlineData("%LAUNCH_TIME:yyyyMMdd_HHmmss%", "19700101_000000")]

        [InlineData("%ID%", "i-abcdefghijk0123456789")]
        [InlineData("%ID:^.*(....)$%", "i-abcdefghijk0123456789")]
        [InlineData("%ID:^.*(....)$;$1%", "6789")]
        [InlineData("%ID:^(.{6}).*(.{4})$;$1_$2%", "i-abcd_6789")]

        public Task TestEval(string expression, string expected)
        {
            var eval = new EC2Evaluator();
            var inst = SampleInstance();

            Assert.Equal(expected, eval.Evaluate(expression, inst));
            return Task.CompletedTask;
        }
    }
}