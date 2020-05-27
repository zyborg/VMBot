using System.Text.Json.Serialization;

namespace Zyborg.VMBot.CWEvents.EC2StateChange
{
    /// <summary>
    /// Represents the strongly-typed detail for an Amazon
    /// <see cref="https://docs.aws.amazon.com/AmazonCloudWatch/latest/events/EventTypes.html#ec2_event_type"
    /// >EC2 State Change Event.
    /// </summary>
    public class EC2StateChangeDetail
    {
        [JsonPropertyName("instance-id")]
        public string InstanceId { get; set; }

        public string State { get; set; }
    }
}
