using System.Configuration;
using System.Diagnostics;
using utility_sample.Core;
using utility_sample.MVVM.Model;

namespace utility_sample.MVVM.ViewModel
{
    /// <summary>
    /// Handles viewmodel for the settings screen
    /// </summary>
    public class SettingsViewModel: ObservableObject
    {
        // Data bindings
        public RelayCommand SaveCommand { get; set; }
        public string ServiceUsername { get; set; }
        public string RootPrincipal { get; set; }
        public string PermissionGroup { get; set; }
        public string AuthnUrl { get; set; }
        public string ConfigDbUrl { get; set; }
        public string DirectoryUrl { get; set; }
        public string MqttUrl { get; set; }

        private FPlusCommunicator _fPlusCommunicator;
        
        /// <summary>
        /// Constructor for making an instance of the settings view
        /// </summary>
        public SettingsViewModel()
        {
            // Better practice might be to bind directly to the ConfigManager Settings from the view
            
            // TODO: Load saved variables if found
            LoadSettings();
            
            _fPlusCommunicator = FPlusCommunicator.GetInstance();
            Debug.Print(_fPlusCommunicator.TestString);
            
            SaveCommand = new RelayCommand(o =>
            {
                // TODO: Attempt to save variables
                Debug.Print(_fPlusCommunicator.TestString);
            });
        }

        private void LoadSettings()
        {
            ServiceUsername = ConfigurationManager.AppSettings.Get("ServiceUsername") ?? string.Empty;
            RootPrincipal = ConfigurationManager.AppSettings.Get("RootPrincipal") ?? string.Empty;
            PermissionGroup = ConfigurationManager.AppSettings.Get("PermissionGroup") ?? string.Empty;
            AuthnUrl = ConfigurationManager.AppSettings.Get("AuthnUrl") ?? string.Empty;
            ConfigDbUrl = ConfigurationManager.AppSettings.Get("ConfigDbUrl") ?? string.Empty;
            DirectoryUrl = ConfigurationManager.AppSettings.Get("DirectoryUrl") ?? string.Empty;
            MqttUrl = ConfigurationManager.AppSettings.Get("MqttUrl") ?? string.Empty;
        }
    }
}
