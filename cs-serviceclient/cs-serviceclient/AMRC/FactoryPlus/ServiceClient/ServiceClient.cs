using AMRC.FactoryPlus.ServiceClient.Constants;
using AMRC.FactoryPlus.ServiceClient.Services;
using Directory = AMRC.FactoryPlus.ServiceClient.Services.Directory;

namespace AMRC.FactoryPlus.ServiceClient;

/// <summary>
/// The ServiceClient holds interfaces with individual F+ services
/// </summary>
public class ServiceClient
{
    /// <summary>
    /// The Auth interface
    /// </summary>
    public readonly Auth Auth;
    /// <summary>
    /// The ConfigDB interface
    /// </summary>
    public readonly ConfigDb ConfigDb;
    /// <summary>
    /// The Directory interface
    /// </summary>
    public readonly Directory Directory;
    /// <summary>
    /// The Discover interface
    /// </summary>
    public readonly Discovery Discovery;
    /// <summary>
    /// The Fetch interface
    /// </summary>
    public readonly FetchClass Fetch;
    /// <summary>
    /// The MQTT interface
    /// </summary>
    public readonly MqttInterface Mqtt;

    /// <summary>
    /// The username to use for Basic auth with services
    /// </summary>
    public string? ServiceUsername;
    /// <summary>
    /// The password to use for Basic auth with services
    /// </summary>
    public string? ServicePassword;
    /// <summary>
    /// The root principal to authenticate
    /// </summary>
    public string? RootPrincipal;
    /// <summary>
    /// The permission group to authenticate
    /// </summary>
    public string? PermissionGroup;
    /// <summary>
    /// The URL of the Authenticate service
    /// </summary>
    public string? AuthnUrl;
    /// <summary>
    /// The URL of the ConfigDB service
    /// </summary>
    public string? ConfigDbUrl;
    /// <summary>
    /// The URL of the Directory service
    /// </summary>
    public string? DirectoryUrl;
    /// <summary>
    /// The URL of the MQTT service
    /// </summary>
    public string? MqttUrl;

    /// <summary>
    /// Creates a ServiceClient that allows for interaction with F+ services.
    /// Provides all the config details at once.
    /// </summary>
    /// <param name="serviceUsername">The username to use for Basic auth with services</param>
    /// <param name="servicePassword">The password to use for Basic auth with services</param>
    /// <param name="rootPrincipal">The root principal to authenticate</param>
    /// <param name="permissionGroup">The permission group to authenticate</param>
    /// <param name="authnUrl">The URL of the Authenticate service</param>
    /// <param name="configDbUrl">The URL of the ConfigDB service</param>
    /// <param name="directoryUrl">The URL of the Directory service</param>
    /// <param name="mqttUrl">The URL of the MQTT service</param>
    public ServiceClient(string? serviceUsername, string? servicePassword, string? rootPrincipal,
                         string? permissionGroup, string? authnUrl, string? configDbUrl, string? directoryUrl, 
                         string? mqttUrl)
    {
        ServiceUsername = serviceUsername;
        ServicePassword = servicePassword;
        RootPrincipal = rootPrincipal;
        PermissionGroup = permissionGroup;
        AuthnUrl = authnUrl;
        ConfigDbUrl = configDbUrl;
        DirectoryUrl = directoryUrl;
        MqttUrl = mqttUrl;
        
        Auth = new Auth(this);
        ConfigDb = new ConfigDb(this);
        Directory = new Directory(this);
        Discovery = new Discovery(this);
        Fetch = new FetchClass(this);
        Mqtt = new MqttInterface(this);
    }

    /// <summary>
    /// Creates a ServiceClient that allows for interaction with F+ services.
    /// Provides only the minimum requirements to use Basic auth and get all URLs from the F+ Directory
    /// </summary>
    /// <param name="serviceUsername">The username to use for Basic auth with services</param>
    /// <param name="servicePassword">The password to use for Basic auth with services</param>
    /// <param name="directoryUrl">The URL of the Directory service</param>
    public ServiceClient(string? serviceUsername, string? servicePassword, string? directoryUrl)
        : this(serviceUsername, servicePassword, null, null, null, null,
            directoryUrl, null) { }

    /// <summary>
    /// Updates the settings that are used for the services
    /// </summary>
    /// <param name="serviceUsername">The username to use for Basic auth with services</param>
    /// <param name="servicePassword">The password to use for Basic auth with services</param>
    /// <param name="rootPrincipal">The root principal to authenticate</param>
    /// <param name="permissionGroup">The permission group to authenticate</param>
    /// <param name="authnUrl">The URL of the Authenticate service</param>
    /// <param name="configDbUrl">The URL of the ConfigDB service</param>
    /// <param name="directoryUrl">The URL of the Directory service</param>
    /// <param name="mqttUrl">The URL of the MQTT service</param>
    public void UpdateConfig(string? serviceUsername, string? servicePassword, string? rootPrincipal,
                             string? permissionGroup, string? authnUrl, string? configDbUrl, string? directoryUrl, 
                             string? mqttUrl)
    {
        ServiceUsername = serviceUsername;
        ServicePassword = servicePassword;
        RootPrincipal = rootPrincipal;
        PermissionGroup = permissionGroup;
        AuthnUrl = authnUrl;
        ConfigDbUrl = configDbUrl;
        DirectoryUrl = directoryUrl;
        MqttUrl = mqttUrl;

        Discovery.SetServiceUrl(UUIDs.Service[ServiceTypes.Authentication], AuthnUrl);
        Discovery.SetServiceUrl(UUIDs.Service[ServiceTypes.ConfigDB], ConfigDbUrl);
        Discovery.SetServiceUrl(UUIDs.Service[ServiceTypes.Directory], DirectoryUrl);
        Discovery.SetServiceUrl(UUIDs.Service[ServiceTypes.MQTT], MqttUrl);
    }
}
