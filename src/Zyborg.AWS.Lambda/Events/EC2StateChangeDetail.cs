using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zyborg.AWS.Lambda.Events
{
    /// <summary>
    /// This detail record should be used in concert with the
    /// <c>Amazon.Lambda.CloudWatchEvents.CloudWatchEvent</c> generic type as its type argument to
    /// target Cloud Watch EC2 State Change events.
    /// Represents the strongly-typed detail for an Amazon
    /// <see cref="https://docs.aws.amazon.com/AmazonCloudWatch/latest/events/EventTypes.html#ec2_event_type"
    /// >EC2 State Change Event.
    /// </summary>
    public class EC2StateChangeDetail
    {
        /// <summary>
        /// The <c>Source</c> property of the <c>CloudWatchEvent</c> will
        /// have this value for an EC2 State Change event.
        /// </summary>
        public const string CloudWatchEventSourceKey = "Source";
        public const string CloudWatchEventSourceValue = "aws.ec2";
        public const string CloudWatchEventDetailTypeKey = "detail-type";
        public const string CloudWatchEventDetailTypeValue = "EC2 Instance State-change Notification";

        [JsonProperty("instance-id")]
        public string InstanceId { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// Defines a <see cref="LambdaEventMatcher" /> that can be used to register
        /// a new Lambda Event Type with a <see cref="LambdaEventDecoder" />.
        /// </summary>
        /// <param name="jtoken"></param>
        /// <returns></returns>
        public static bool Matcher(JToken jtoken)
        {
            var match = jtoken.SelectToken(CloudWatchEventSourceKey)?.ToString() == CloudWatchEventSourceValue
                && jtoken.SelectToken(CloudWatchEventDetailTypeKey)?.ToString() == CloudWatchEventDetailTypeValue
                && !string.IsNullOrEmpty(jtoken.SelectToken("Detail.State")?.ToString());
            return match;
        }
    }
}
