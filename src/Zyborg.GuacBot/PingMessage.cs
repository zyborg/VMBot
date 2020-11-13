using System;
using System.Threading.Tasks;
using NServiceBus;

namespace Zyborg.GuacBot
{
    public class PingMessage : IMessage
    {
        public string Message { get; set; }
    }

    public class PingMessageHandler : IHandleMessages<PingMessage>
    {
        public PingMessageHandler()
        {
            Console.WriteLine("Constructing new PING Message Handler");
        }

        public Task Handle(PingMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Handling Queued PING Message Invocation: {message.Message}");
            return Task.CompletedTask;
        }
    }
}