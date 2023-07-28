namespace AMRC.FactoryPlus.ServiceClient;

/// <summary>
/// The ServiceClient holds interfaces with individual F+ services
/// </summary>
public class ServiceClient
{
    public Auth Auth;
    public ConfigDb ConfigDb;
    public Directory Directory;
    public Discovery Discovery;
    public FetchClass Fetch;
    public MQTTInterface Mqtt;

    public string? ServiceUsername;
    public string? ServicePassword;
    public string? RootPrincipal;
    public string? PermissionGroup;
    public string? DirectoryUrl;

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
    }
}
