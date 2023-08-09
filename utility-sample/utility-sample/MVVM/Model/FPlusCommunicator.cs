using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using AMRC.FactoryPlus.ServiceClient;
using Com.Cirruslink.Sparkplug.Protobuf;
using MQTTnet;
using MQTTnet.Client;

namespace utility_sample.MVVM.Model
{
    /// <summary>
    /// Handles getting and putting of data from Factory+
    /// </summary>
    public class FPlusCommunicator
    {
        private static FPlusCommunicator _instance;
        
        private string _serviceUsername;
        private string _servicePassword;
        private string _rootPrincipal;
        private string _permissionGroup;
        private string _authnUrl;
        private string _configDbUrl;
        private string _directoryUrl;
        private string _mqttUrl;

        private ServiceClient _serviceClient;
        private IMqttClient _mqttClient;

        /// <summary>
        /// Gets the current instance of this class
        /// </summary>
        /// <returns>FPlusCommunicator instance</returns>
        public static FPlusCommunicator GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FPlusCommunicator();
                _instance.LoadSettings();
            }
            return _instance;
        }

        /// <summary>
        /// Loads settings from configuration manager
        /// </summary>
        public void LoadSettings()
        {
            _rootPrincipal = ConfigurationManager.AppSettings.Get("RootPrincipal") ?? string.Empty;
            _permissionGroup = ConfigurationManager.AppSettings.Get("PermissionGroup") ?? string.Empty;
            _authnUrl = ConfigurationManager.AppSettings.Get("AuthnUrl") ?? string.Empty;
            _configDbUrl = ConfigurationManager.AppSettings.Get("ConfigDbUrl") ?? string.Empty;
            _directoryUrl = ConfigurationManager.AppSettings.Get("DirectoryUrl") ?? string.Empty;
            _mqttUrl = ConfigurationManager.AppSettings.Get("MqttUrl") ?? string.Empty;
        }

        public async void StartFPlusStuff(string username, string password)
        {
            _serviceClient = new ServiceClient();
            // var test = await serviceClient.ConfigDb.Search(UUIDs.App[AppSubcomponents.Info], new Dictionary<string, object>(), new Dictionary<string, string>(), null);
            
            _mqttClient = await _serviceClient.Mqtt.GetMqttClient();

            _serviceClient.Mqtt.OnMessageReceived += MessageReceived;

            await _mqttClient.SubscribeAsync("spBv1.0/#");
            // MessageBox.Show(test.Length.ToString());
        }
        
        public async void StopFPlusStuff()
        {
            await _mqttClient.UnsubscribeAsync("spBv1.0/#");
            await _mqttClient.DisconnectAsync();
        }

        private void MessageReceived(IMqttClient mqttClient, Payload payload, MqttApplicationMessage mqttMessageObject)
        {
            if (mqttClient != _mqttClient)
            {
                return;
            }
            Debug.WriteLine($"Message received on: {mqttMessageObject.Topic}");
            Debug.WriteLine(payload.Metrics.Count);
            Debug.WriteLine(payload.Metrics);
        }
    }
}
