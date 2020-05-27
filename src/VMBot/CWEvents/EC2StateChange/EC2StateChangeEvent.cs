using Amazon.Lambda.CloudWatchEvents;

namespace Zyborg.VMBot.CWEvents.EC2StateChange
{
    public class EC2StateChangeEvent : CloudWatchEvent<EC2StateChangeDetail>
    {
        /// <summary>
        /// The <c>Source</c> property of the <c>CloudWatchEvent</c> will
        /// have this value for an EC2 State Change event.
        /// </summary>
        public const string CloudWatchEventSource = "aws.ec2";        
    }
}