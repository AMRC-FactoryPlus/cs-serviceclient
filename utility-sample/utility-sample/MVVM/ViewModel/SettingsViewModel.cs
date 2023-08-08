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
            // TODO: Load saved variables if found
            _fPlusCommunicator = FPlusCommunicator.GetInstance();
            _fPlusCommunicator.TestString = "Hello from Settings View";
            Debug.Print(_fPlusCommunicator.TestString);
            
            SaveCommand = new RelayCommand(o =>
            {
                // TODO: Attempt to save variables
                _fPlusCommunicator.TestString = "Hello from Save button";
                Debug.Print(_fPlusCommunicator.TestString);
            });
        }

        private void LoadSettings()
        {
            
        }
    }
}
