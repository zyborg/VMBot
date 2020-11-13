using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Zyborg.AWS.Lambda.Events;

namespace Zyborg.GuacBot
{
    public class QueuedMessageHandler : IHandleMessages<EC2StateChangeDetail>
    {
        // private ILogger _logger;

        // public QueuedMessageHandler(ILogger<QueuedMessageHandler> logger)
        // {
        //     _logger = logger;
        // }

        private NServiceBus.Logging.ILog _logger = NServiceBus.Logging.LogManager.GetLogger<QueuedMessageHandler>();

        public Task Handle(EC2StateChangeDetail message, IMessageHandlerContext context)
        {
            //_logger.LogInformation("Handling Queued Message Invocation");
            _logger.Info("Handling Queued Message Invocation");
            return Task.CompletedTask;
        }
    }
}