using System.Text.Json;
using Amazon.Lambda.CloudWatchEvents;

namespace Zyborg.VMBot.CWEvents.EC2AutoScale
{
    public class EC2AutoScaleEvent : CloudWatchEvent<JsonElement>
    {
        public bool IsEC2InstanceLaunchLifecycleAction() =>
            string.Equals(DetailType, EC2AutoScaleDetailTypes.EC2InstanceLaunchLifecycleAction);
        public bool IsEC2InstanceLaunchSuccessful() =>
            string.Equals(DetailType, EC2AutoScaleDetailTypes.EC2InstanceLaunchSuccessful);
        public bool IsEC2InstanceLaunchUnsuccessful() =>
            string.Equals(DetailType, EC2AutoScaleDetailTypes.EC2InstanceLaunchUnsuccessful);
        public bool IsEC2InstanceTerminateLifecycleAction() =>
            string.Equals(DetailType, EC2AutoScaleDetailTypes.EC2InstanceTerminateLifecycleAction);
        public bool IsEC2InstanceTerminateSuccessful() =>
            string.Equals(DetailType, EC2AutoScaleDetailTypes.EC2InstanceTerminateSuccessful);
        public bool IsEC2InstanceTerminateUnsuccessful() =>
            string.Equals(DetailType, EC2AutoScaleDetailTypes.EC2InstanceTerminateUnsuccessful);

        public InstanceLifecycleHookDetail GetDetailAsLifecycleHook()
        {
            // Unfortunately this the best way to do this for now, until
            // this is resolved:  https://github.com/dotnet/runtime/issues/31274
            return JsonSerializer.Deserialize<InstanceLifecycleHookDetail>(this.Detail.GetRawText());
        }

        public InstanceStateChangeDetail GetDetailAsStateChange()
        {
            // Unfortunately this the best way to do this for now, until
            // this is resolved:  https://github.com/dotnet/runtime/issues/31274
            return JsonSerializer.Deserialize<InstanceStateChangeDetail>(this.Detail.GetRawText());
        }
    }

    public class EC2AutoScaleEvent<T> : CloudWatchEvent<T> where T : IEC2AutoScaleDetail
    {
        /// <summary>
        /// The <c>Source</c> property of the <c>CloudWatchEvent</c> will
        /// have this value for an EC2 Auto Scale event.
        /// </summary>
        public const string CloudWatchEventSource = "aws.autoscaling";
    }

    public class EC2AutoScaleDetailTypes
    {
        public const string EC2InstanceLaunchLifecycleAction =  "EC2 Instance-launch Lifecycle Action";
        public const string EC2InstanceLaunchSuccessful = "EC2 Instance Launch Successful";
        public const string EC2InstanceLaunchUnsuccessful = "EC2 Instance Launch Unsuccessful";
        public const string EC2InstanceTerminateLifecycleAction = "EC2 Instance-terminate Lifecycle Action";
        public const string EC2InstanceTerminateSuccessful = "EC2 Instance Terminate Successful";
        public const string EC2InstanceTerminateUnsuccessful = "EC2 Instance Terminate Unsuccessful";
    }
}