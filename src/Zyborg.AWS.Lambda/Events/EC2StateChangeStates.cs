namespace Zyborg.AWS.Lambda.Events
{
    public class EC2StateChangeStates
    {
        public const string Pending = "pending";
        public const string ShuttingDown = "shutting-down";
        public const string Running = "running";
        public const string Stopped = "stopped";
        public const string Stopping = "stopping";
        public const string Terminated = "terminated";
    }
}
