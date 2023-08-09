using System.Diagnostics;
using AMRC.FactoryPlus.ServiceClient;
using Com.Cirruslink.Sparkplug.Protobuf;
using MQTTnet;
using MQTTnet.Client;
using utility_sample.Core;
using utility_sample.MVVM.Model;

namespace utility_sample.MVVM.ViewModel
{
    /// <summary>
    /// Handles viewmodel for the default home view
    /// </summary>
    public class HomeViewModel: ObservableObject
    {
        public RelayCommand SubmitCommand { get; set; }
        
        public string ServiceUsername { get; set; }
        public string ServicePassword { get; set; }
        public RelayCommand CancelCommand { get; set; }

        private FPlusCommunicator _fPlusCommunicator;
        private ServiceClient _serviceClient;
        private IMqttClient _mqttClient;
        
        /// <summary>
        /// Constructor for making an instance of the settings view model
        /// </summary>
        public HomeViewModel()
        {
            _fPlusCommunicator = FPlusCommunicator.GetInstance();
            
            SubmitCommand = new RelayCommand(o =>
            {
                if (ServiceUsername != null && ServicePassword != null)
                {
                    _fPlusCommunicator.DoTest(ServiceUsername, ServicePassword);
                }
                StartFPlusStuff();
            });
            
            CancelCommand = new RelayCommand(o =>
            {
                StopFPlusStuff();
            });
        }
        
        private async void StartFPlusStuff()
        {
            _serviceClient = new ServiceClient();
            // var test = await serviceClient.ConfigDb.Search(UUIDs.App[AppSubcomponents.Info], new Dictionary<string, object>(), new Dictionary<string, string>(), null);
            
            _mqttClient = await _serviceClient.Mqtt.GetMqttClient();

            _serviceClient.Mqtt.OnMessageReceived += MessageReceived;

            await _mqttClient.SubscribeAsync("spBv1.0/#");
            // MessageBox.Show(test.Length.ToString());
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

        private async void StopFPlusStuff()
        {
            await _mqttClient.UnsubscribeAsync("spBv1.0/#");
            await _mqttClient.DisconnectAsync();
        }
    }
}
