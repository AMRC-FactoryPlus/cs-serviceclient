using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
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
        public string RootPrincipal { get; set; }
        public string PermissionGroup { get; set; }
        public string AuthnUrl { get; set; }
        public string ConfigDbUrl { get; set; }
        public string DirectoryUrl { get; set; }
        public string MqttUrl { get; set; }

        // FPlus instance
        private FPlusCommunicator _fPlusCommunicator;
        
        /// <summary>
        /// Constructor for making an instance of the settings view
        /// </summary>
        public SettingsViewModel()
        {
            // Better practice might be to bind directly to the ConfigManager Settings from the view
            
            // TODO: Load saved variables if found
            PopulateSettings();
            
            _fPlusCommunicator = FPlusCommunicator.GetInstance();
            
            SaveCommand = new RelayCommand(o =>
            {
                SaveSettings();
            });
        }

        private void SaveSettings()
        {
            try
            {
                UpdateSetting("RootPrincipal", RootPrincipal);
                UpdateSetting("PermissionGroup", PermissionGroup);
                UpdateSetting("AuthnUrl", AuthnUrl);
                UpdateSetting("ConfigDbUrl", ConfigDbUrl);
                UpdateSetting("DirectoryUrl", DirectoryUrl);
                UpdateSetting("MqttUrl", MqttUrl);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while saving " + e.ToString());
                Debug.WriteLine(e);
                return;
            }
            
            MessageBox.Show("Settings Saved");

            // Refresh FPlus instance
            _fPlusCommunicator.LoadSettings();
        }

        private void UpdateSetting(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);  
            var settings = configFile.AppSettings.Settings;  
            if (settings[key] == null)  
            {  
                settings.Add(key, value);  
            }  
            else  
            {  
                settings[key].Value = value;  
            }  
            configFile.Save(ConfigurationSaveMode.Modified);  
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
        
        private void PopulateSettings()
        {
            // Populate UI elements with found data
            RootPrincipal = ConfigurationManager.AppSettings.Get("RootPrincipal") ?? string.Empty;
            PermissionGroup = ConfigurationManager.AppSettings.Get("PermissionGroup") ?? string.Empty;
            AuthnUrl = ConfigurationManager.AppSettings.Get("AuthnUrl") ?? string.Empty;
            ConfigDbUrl = ConfigurationManager.AppSettings.Get("ConfigDbUrl") ?? string.Empty;
            DirectoryUrl = ConfigurationManager.AppSettings.Get("DirectoryUrl") ?? string.Empty;
            MqttUrl = ConfigurationManager.AppSettings.Get("MqttUrl") ?? string.Empty;
        }
    }
}
