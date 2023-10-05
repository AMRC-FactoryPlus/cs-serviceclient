using System;
using System.Threading;
using System.Threading.Tasks;
using AMRC.FactoryPlus.ServiceClient.Constants;
using Com.Cirruslink.Sparkplug.Protobuf;
using Cysharp.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace AMRC.FactoryPlus.ServiceClient.Services
{

    /// <summary>
    /// The MQTT service interface
    /// </summary>
    public class MqttInterface : ServiceInterface
    {
        /// <summary>
        /// The type of handler method required when an MQTT message is received.
        /// </summary>
        /// <param name="mqttClient">The MqttClient that the message came to</param>
        /// <param name="payload">The Payload of the message, decided from Sparkplug protobufs for consumption</param>
        /// <param name="message">The raw MQTT message, which will likely be encoded in protobufs</param>
        public delegate void MessageReceivedHandler(IMqttClient mqttClient, Payload payload,
                                                    MqttApplicationMessage message);

        /// <summary>
        /// Event fired when a message is received a connected MqttClient.
        /// This can be handled externally, in user-code, but this helper abstracts away some concepts.
        /// The Google.Protobuf library and Sparkplug proto classes are used to decode the incoming Payload.
        /// This makes it easier to consume the result (i.e. the Metrics, or other data).
        /// The raw MqttApplicationMessage is provided for user-code to interact with if required.
        /// </summary>
        public event MessageReceivedHandler OnMessageReceived;


        /// <inheritdoc />
        public MqttInterface(ServiceClient serviceClient) : base(serviceClient)
        {
            ServiceType = ServiceTypes.MQTT;
        }

        /// <summary>
        /// Sets up an MQTTClient connection
        /// </summary>
        /// <returns>A connected instance of an MQTTClient</returns>
        /// <exception cref="Exception"></exception>
        public async UniTask<IMqttClient> GetMqttClient(string? hostUrl = "")
        {
            var url = hostUrl ?? "";
            if (String.IsNullOrEmpty(url))
            {
                url = await ServiceClient.Discovery.ServiceUrl(UUIDs.Service[ServiceTypes.MQTT]);
            }

            if (String.IsNullOrEmpty(url))
            {
                throw new Exception("No host provided and no url could be found");
            }

            if (!String.IsNullOrEmpty(ServiceClient.ServiceUsername) &&
                !String.IsNullOrEmpty(ServiceClient.ServicePassword))
            {
                return await BasicClient(url, ServiceClient.ServiceUsername, ServiceClient.ServicePassword);
            }

            throw new Exception("No username or password available");
        }

        private async UniTask<IMqttClient> BasicClient(string url, string username, string password)
        {
            var mqttFactory = new MqttFactory();
            var mqttClient = mqttFactory.CreateMqttClient();

            var hostUri = new Uri(url);
            var mqttClientOptions = new MqttClientOptionsBuilder()
                                    // TODO: .WithClientId()
                                    .WithTcpServer(hostUri.Host, hostUri.Port > 0 ? hostUri.Port : (int?) null)
                                    .WithCredentials(username, password)
                                    .WithTls()
                                    // TODO: .WithCleanStart()
                                    .Build();

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var payload = Payload.Parser.ParseFrom(e.ApplicationMessage.PayloadSegment);
                OnMessageReceived.Invoke(mqttClient, payload, e.ApplicationMessage);
                return Task.CompletedTask;
            };

            var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            if (response == null || response.ResultCode != MqttClientConnectResultCode.Success)
            {
                throw new Exception($"Failed to connect to MQTT server {response?.ResultCode.ToString()}");
            }

            return mqttClient;
        }
    }
}
