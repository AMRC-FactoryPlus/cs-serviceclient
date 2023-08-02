namespace AMRC.FactoryPlus.ServiceClient;

/// <summary>
/// The ServiceClient holds interfaces with individual F+ services
/// </summary>
public class ServiceClient
{
    public readonly Auth Auth;
    public readonly ConfigDb ConfigDb;
    public readonly Directory Directory;
    public readonly Discovery Discovery;
    public readonly FetchClass Fetch;
    public readonly MQTTInterface Mqtt;

    public readonly string? ServiceUsername;
    public readonly string? ServicePassword;
    public readonly string? RootPrincipal;
    public readonly string? PermissionGroup;
    public readonly string? AuthnUrl;
    public readonly string? ConfigDbUrl;
    public readonly string? DirectoryUrl;
    public readonly string? MqttUrl;

    /// <summary>
    /// Creates a ServiceClient that allows for interaction with F+ services
    /// </summary>
    public ServiceClient()
    {
        Auth = new Auth(this);
        ConfigDb = new ConfigDb(this);
        Directory = new Directory(this);
        Discovery = new Discovery(this);
        Fetch = new FetchClass(this);
        Mqtt = new MQTTInterface(this);

        ServiceUsername = Environment.GetEnvironmentVariable("ServiceUsername");
        ServicePassword = Environment.GetEnvironmentVariable("ServicePassword");
        RootPrincipal = Environment.GetEnvironmentVariable("RootPrincipal");
        PermissionGroup = Environment.GetEnvironmentVariable("PermissionGroup");
        AuthnUrl = Environment.GetEnvironmentVariable("AuthnUrl");
        ConfigDbUrl = Environment.GetEnvironmentVariable("ConfigDbUrl");
        DirectoryUrl = Environment.GetEnvironmentVariable("DirectoryUrl");
        MqttUrl = Environment.GetEnvironmentVariable("MqttUrl");
    }
}
