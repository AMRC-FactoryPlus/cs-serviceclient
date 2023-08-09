using System.Collections.Generic;
using System.Configuration;
using AMRC.FactoryPlus.ServiceClient;

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

        /// <summary>
        /// TODO
        /// </summary>
        public async void DoTest(string username, string password)
        {
            var test = await _serviceClient.ConfigDb.Search(UUIDs.App[AppSubcomponents.Info], new Dictionary<string, object>(), new Dictionary<string, string>(), null);
        }
    }
}
