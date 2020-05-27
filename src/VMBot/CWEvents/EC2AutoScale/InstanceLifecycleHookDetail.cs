namespace Zyborg.VMBot.CWEvents.EC2AutoScale
{
    /// <summary>
    /// The strongly-typed detail for an EC2 Auto Scale Event for an instance
    /// moving to "Pending:Wait" state due to a lifecycle hook.
    /// 
    /// <see cref="https://docs.aws.amazon.com/autoscaling/ec2/userguide/cloud-watch-events.html#launch-lifecycle-action"
    /// >EC2 Instance Launch Lifecycle Action</see>
    /// <see cref="https://docs.aws.amazon.com/autoscaling/ec2/userguide/cloud-watch-events.html#terminate-lifecycle-action"
    /// >EC2 Instance Terminate Lifecycle Action</see>
    /// </summary>
    public class InstanceLifecycleHookDetail : IEC2AutoScaleDetail
    {
        // Sample:  "LifecycleActionToken": "87654321-4321-4321-4321-210987654321"
        public string LifecycleActionToken { get; set; }

        // Sample:  "AutoScalingGroupName": "my-asg"
        public string AutoScalingGroupName { get; set; }

        // Sample:  "LifecycleHookName": "my-lifecycle-hook"
        public string LifecycleHookName { get; set; }

        // Sample:  "EC2InstanceId": "i-1234567890abcdef0"
        public string EC2InstanceId { get; set; }

        // Sample:  "LifecycleTransition": "autoscaling:EC2_INSTANCE_LAUNCHING"
        public string LifecycleTransition { get; set; }

        // Sample:  "NotificationMetadata": "additional-info"
        public string NotificationMetadata { get; set; }
    }

    public class LifecycleTransitions
    {
        public const string PendingWait = "autoscaling:EC2_INSTANCE_LAUNCHING";
        public const string TerminatingWait = "autoscaling:EC2_INSTANCE_TERMINATING";
    }
}
