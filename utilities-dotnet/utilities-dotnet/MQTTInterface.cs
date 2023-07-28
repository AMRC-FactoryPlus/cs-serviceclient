using Cysharp.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace AMRC.FactoryPlus.ServiceClient;

/// <summary>
/// The MQTT service interface
/// </summary>
public class MQTTInterface : ServiceInterface
{
    /// <inheritdoc />
    public MQTTInterface(ServiceClient serviceClient) : base(serviceClient)
    {
        _serviceType = ServiceTypes.MQTT;
    }

    /// <summary>
    /// Sets up an MQTTClient connection
    /// </summary>
    /// <returns>A connected instance of an MQTTClient</returns>
    /// <exception cref="Exception"></exception>
    public async UniTask<IMqttClient> GetMqttClient()
    {
        // TODO: Complete method
        // Find URL
        var url = "";
        if (!String.IsNullOrWhiteSpace(_serviceClient.ServiceUsername) &&
            !String.IsNullOrWhiteSpace(_serviceClient.ServicePassword))
        {
            return await BasicClient(url, _serviceClient.ServiceUsername, _serviceClient.ServicePassword);
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
