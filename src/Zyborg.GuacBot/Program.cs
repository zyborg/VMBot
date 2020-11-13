using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using NServiceBus;

namespace Zyborg.GuacBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Setup();

            //await CreateSQS().SendMessageAsync("GuacBot", "{ \"Message\": \"Hello WORLD!\" }");

            await SendMessage();
        }

        private static CredentialProfile _credProfile;
        private static IEndpointInstance _sqsEndpoint;

        static async Task Setup()
        {
            new NetSDKCredentialsFile().TryGetProfile("auto@aws1", out _credProfile);

            var endpointConfiguration = new EndpointConfiguration("GuacBot-Sender");
            endpointConfiguration.SendFailedMessagesTo("GuacBot-Errors");
            // endpointConfiguration.UsePersistence<InMemoryPersistence>();
            
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();


            var transport = endpointConfiguration.UseTransport<SqsTransport>();
            transport.ClientFactory(CreateSQS);
            transport.ClientFactory(CreateSNS);

            //transport.Routing().RouteToEndpoint(typeof(PingMessage), "GuacBot");

            _sqsEndpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        }

        static IAmazonSQS CreateSQS() =>
            new AmazonSQSClient(_credProfile.GetAWSCredentials(null), RegionEndpoint.USEast1);
        static IAmazonSimpleNotificationService CreateSNS() =>
            new AmazonSimpleNotificationServiceClient(_credProfile.GetAWSCredentials(null), RegionEndpoint.USEast1);

        static async Task SendMessage()
        {
            await _sqsEndpoint.Send("GuacBot", new PingMessage
            {
                Message = "Hello World",
            }).ConfigureAwait(false);
        }
    }
}