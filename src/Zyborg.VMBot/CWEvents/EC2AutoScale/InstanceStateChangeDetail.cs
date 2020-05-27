using System.Text.Json.Serialization;

namespace Zyborg.VMBot.CWEvents.EC2AutoScale
{
    public class InstanceStateChangeDetail : IEC2AutoScaleDetail
    {
        /// One of the possible values defined in
        /// <see cref="InstanceStateChangeDetail#StatusCodes">.
        // Sample:  "StatusCode": "InProgress"
        public string StatusCode { get; set; }
        
        // Sample:  "Description": "Launching a new EC2 instance: i-12345678"
        public string Description { get; set; }
        
        // Sample:  "AutoScalingGroupName": "my-auto-scaling-group"
        public string AutoScalingGroupName { get; set; }
        
        // Sample:  "ActivityId": "87654321-4321-4321-4321-210987654321"
        public string ActivityId { get; set; }
        
        // Sample:  "Details": {}
        public MoreDetails Details { get; set; }
        
        // Sample:  "RequestId": "12345678-1234-1234-1234-123456789012"
        public string RequestId { get; set; }
        
        // Sample:  "StatusMessage": ""
        public string StatusMessage { get; set; }
        
        // Sample:  "EndTime": "yyyy-mm-ddThh:mm:ssZ"
        public string EndTime { get; set; }
        
        // Sample:  "EC2InstanceId": "i-1234567890abcdef0"
        public string EC2InstanceId { get; set; }
        
        // Sample:  "StartTime": "yyyy-mm-ddThh:mm:ssZ"
        public string StartTime { get; set; }
        
        // Sample:  "Cause": "description-text"        
        public string Cause { get; set; }

        /// <summary>
        /// The set of valid values for
        /// <see cref="InstanceStateChangeDetail#StatusCode">.
        /// </summary>
        public class StatusCodes
        {
            public const string Failed = "Failed";
            public const string InProgress = "InProgress";
        }

        public class MoreDetails
        {
            // Sample:  "Availability Zone": "us-west-2b"
            [JsonPropertyName("Availability Zone")]
            public string AvailabilityZone { get; set; }

            // Sample:  "Subnet ID": "subnet-12345678"
            [JsonPropertyName("Subnet ID")]
            public string SubnetId { get; set; }
        }
    }
}