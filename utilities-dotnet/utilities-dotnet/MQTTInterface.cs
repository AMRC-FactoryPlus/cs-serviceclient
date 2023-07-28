using MQTTnet;
using MQTTnet.Client;

namespace AMRC.FactoryPlus.ServiceClient;

public class MQTTInterface : ServiceInterface
{
    public MQTTInterface(ServiceClient serviceClient) : base(serviceClient)
    {
        _serviceType = ServiceTypes.MQTT;
    }

    public async Task<IMqttClient> GetMqttClient()
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

    private async Task<IMqttClient> BasicClient(string url, string username, string password)
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
