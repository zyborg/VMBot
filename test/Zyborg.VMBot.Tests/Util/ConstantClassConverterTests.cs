using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Route53;
using Amazon.Route53.Model;
using Xunit;
using FluentAssertions;
using Zyborg.VMBot.Util;

namespace Zyborg.VMBot.Tests.Util
{
    public class ConstantClassConverterTests
    {
        static readonly JsonSerializerOptions SerOptions;
        
        static ConstantClassConverterTests()
        {
            SerOptions = new JsonSerializerOptions();
            SerOptions.Converters.Add(new ConstantClassConverter());
        }

        [Fact]
        public void convert_simple_value_to_json()
        {
            var expected = @$"""{CloudWatchRegion.UsGovEast1.Value}""";
            var actual = JsonSerializer.Serialize(CloudWatchRegion.UsGovEast1, SerOptions);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void convert_simple_null_to_json()
        {
            var expected = @$"null";
            var actual = JsonSerializer.Serialize((CloudWatchRegion)null, SerOptions);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void convert_simple_value_from_json()
        {
            var expected = CloudWatchRegion.UsGovEast1;
            var actual = JsonSerializer.Deserialize<CloudWatchRegion>(
                @$"""{CloudWatchRegion.UsGovEast1.Value}""", SerOptions);
            Assert.Equal(expected, actual);
            Assert.Equal(expected.Value, actual.Value);
        }

        [Fact]
        public void convert_simple_null_from_json()
        {
            var expected = (CloudWatchRegion)null;
            var actual = JsonSerializer.Deserialize<CloudWatchRegion>("null", SerOptions);
            Assert.Equal(expected, actual);
        }

        class MyType1
        {
            public CloudWatchRegion Region { get; set; }
        }

        [Fact]
        public void convert_simple_type_with_value()
        {
            var sample = new MyType1 { Region = CloudWatchRegion.AfSouth1 };
            var expected = @$"{{""Region"":""{CloudWatchRegion.AfSouth1}""}}";

            var json = JsonSerializer.Serialize(sample, SerOptions);
            var inst = JsonSerializer.Deserialize<MyType1>(json, SerOptions);

            Assert.Equal(expected, json);
            Assert.Equal(sample.Region, inst.Region);
            Assert.Equal(sample.Region.Value, inst.Region.Value);
        }

        [Fact]
        public void convert_simple_type_with_null()
        {
            var sample = new MyType1 { Region = null };
            var expected = @$"{{""Region"":null}}";

            var json = JsonSerializer.Serialize(sample, SerOptions);
            var inst = JsonSerializer.Deserialize<MyType1>(json, SerOptions);

            Assert.Equal(expected, json);
            Assert.Equal(sample.Region, inst.Region);
        }

        [Fact]
        public void convert_HealthCheckConfig_to_json()
        {
            var hcc = new HealthCheckConfig
            {
                AlarmIdentifier = new AlarmIdentifier
                {
                    Name = "AlarmId",
                    Region = CloudWatchRegion.ApNortheast1,
                },
                ChildHealthChecks = new List<string> { "Child1", "Child2", },
                Disabled = true,
                EnableSNI = true,
                FailureThreshold = 99,
                FullyQualifiedDomainName = "FQDN1",
                HealthThreshold = 88,
                InsufficientDataHealthStatus = InsufficientDataHealthStatus.LastKnownStatus,
                Inverted = true,
                IPAddress = "1.2.3.4",
                MeasureLatency = true,
                Port = 7654,
                Regions = new List<string> { "reg1", "reg2", "reg3", },
                RequestInterval = 55,
                ResourcePath = "ResPath1",
                SearchString = "Search1",
                Type = HealthCheckType.CLOUDWATCH_METRIC,
            };

            var json = JsonSerializer.Serialize(hcc, SerOptions);
            var orig = JsonSerializer.Deserialize<HealthCheckConfig>(json, SerOptions);

            orig.Should().BeEquivalentTo(hcc);
        }        
    }
}