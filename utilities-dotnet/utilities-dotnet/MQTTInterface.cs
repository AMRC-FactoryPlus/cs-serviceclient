using Cysharp.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace AMRC.FactoryPlus.ServiceClient;

/// <summary>
/// The MQTT service interface
/// </summary>
public class MqttInterface : ServiceInterface
{
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
    public async UniTask<IMqttClient> GetMqttClient(string? host)
    {
        var url = host ?? "";
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
        using var mqttClient = mqttFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder()
                                // TODO: .WithClientId()
                                .WithTcpServer(url)
                                .WithCredentials(username, password)
                                .WithTls()
                                // TODO: .WithCleanStart()
                                .Build();
            
        var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        if (response == null || response.ResultCode != MqttClientConnectResultCode.Success)
        {
            throw new Exception($"Failed to connect to MQTT server {response?.ResultCode.ToString()}");
        }
        return mqttClient;
    }
}
