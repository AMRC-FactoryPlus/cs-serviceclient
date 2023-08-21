using System;
using System.Configuration;
using System.Diagnostics;
using System.Security.Authentication;
using System.Windows;
using AMRC.FactoryPlus.ServiceClient;
using Com.Cirruslink.Sparkplug.Protobuf;
using Flurl.Http;
using MQTTnet;
using MQTTnet.Client;

namespace utility_sample.MVVM.Model
{
    /// <summary>
    /// Handles getting and putting of data from Factory+
    /// </summary>
    public class FPlusCommunicator
    {
        public delegate void DistributeMessageHandler(MqttApplicationMessage mqttMessageObject, Payload payload);
        public event DistributeMessageHandler DistributeMessage;

        private static FPlusCommunicator _instance;
        
        private string _rootPrincipal;
        private string _permissionGroup;
        private string _authnUrl;
        private string _configDbUrl;
        private string _directoryUrl;
        private string _mqttUrl;

        private ServiceClient _serviceClient;
        private IMqttClient? _mqttClient = null;

        /// <summary>
        /// Gets the current instance of this class
        /// </summary>
        /// <returns>FPlusCommunicator instance</returns>
        public static FPlusCommunicator GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FPlusCommunicator();
            }
            return _instance;
        }

        public FPlusCommunicator()
        {
            _serviceClient = new ServiceClient(null,null,null);
            LoadSettings();
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

        public async void StartFPlusStuff(string username, string password, string topic)
        {
            _serviceClient.UpdateConfig(username, password, _rootPrincipal, _permissionGroup, _authnUrl, _configDbUrl, _directoryUrl, _mqttUrl);

            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                try
                {
                    _mqttClient = await _serviceClient.Mqtt.GetMqttClient();
                }
                catch (AuthenticationException)
                {
                    MessageBox.Show("The username or password was incorrect");
                    return;
                }
                catch (FlurlHttpTimeoutException e)
                {
                    MessageBox.Show($"Request to {e.Call.Request.Url} timed out, is the server contactable?");
                    return;
                }
                catch (FlurlHttpException e)
                {
                    MessageBox.Show($"Request to {e.Call.Request.Url} failed");
                    return;
                }
                _serviceClient.Mqtt.OnMessageReceived += MessageReceived;
            }
            
            await _mqttClient.SubscribeAsync(topic);
        }
        
        public async void StopFPlusStuff(string topic)
        {
            await _mqttClient.DisconnectAsync();
        }

        private void MessageReceived(IMqttClient mqttClient, Payload payload, MqttApplicationMessage mqttMessageObject)
        {
            if (mqttClient != _mqttClient)
            {
                return;
            }
            // Debug.WriteLine($"Message received on: {mqttMessageObject.Topic}");
            // Debug.WriteLine(payload.Metrics.Count);
            // Debug.WriteLine(payload.Metrics);
            
            DistributeMessage?.Invoke(mqttMessageObject, payload);
        }
    }
}
