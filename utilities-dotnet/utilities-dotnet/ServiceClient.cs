namespace AMRC.FactoryPlus.ServiceClient;

public class ServiceClient
{
    // TODO: public Auth Auth;
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

    public ServiceClient()
    {
        // TODO: Auth = new Auth(this);
        ConfigDb = new ConfigDb(this);
        Directory = new Directory(this);
        Discovery = new Discovery(this);
        Fetch = new FetchClass(this);
        Mqtt = new MQTTInterface(this);
    }
}
