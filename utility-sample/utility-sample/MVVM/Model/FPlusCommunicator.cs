namespace utility_sample.MVVM.Model
{
    /// <summary>
    /// Handles getting and putting of data from Factory+
    /// </summary>
    public class FPlusCommunicator
    {
        private string _serviceUsername;
        private string _rootPrincipal;
        private string _permissionGroup;
        private string _authnUrl;
        private string _configDbUrl;
        private string _directoryUrl;
        private string _mqttUrl;

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="serviceUsername"></param>
        /// <param name="rootPrincipal"></param>
        /// <param name="permissionGroup"></param>
        /// <param name="authnUrl"></param>
        /// <param name="configDbUrl"></param>
        /// <param name="directoryUrl"></param>
        /// <param name="mqttUrl"></param>
        public FPlusCommunicator(string serviceUsername, string rootPrincipal, string permissionGroup, string authnUrl,
                                 string configDbUrl, string directoryUrl, string mqttUrl)
        {
            _serviceUsername = serviceUsername;
            _rootPrincipal = rootPrincipal;
            _permissionGroup = permissionGroup;
            _authnUrl = authnUrl;
            _configDbUrl = configDbUrl;
            _directoryUrl = directoryUrl;
            _mqttUrl = mqttUrl;
        }
    }
}
