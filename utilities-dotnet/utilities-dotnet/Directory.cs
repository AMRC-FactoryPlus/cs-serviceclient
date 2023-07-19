namespace AMRC.FactoryPlus.ServiceClient;

public class Directory : ServiceInterface
{
    public Directory(ServiceClient serviceClient) : base(serviceClient)
    {
        
    }
    
    public async Task<string[]> ServiceUrls(string service)
    {
        // TODO: Fetch stuff
        return new[] { "" };
    }

    public void RegisterServiceUrl(string service, string url)
    {
        // TODO: PUT stuff
    }
}
