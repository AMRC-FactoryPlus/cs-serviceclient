namespace utilities_dotnet;

public class Discovery : ServiceInterface
{
    public Discovery() : base()
    {
        
    }

    public async Task<string[]> FindServiceUrls(string service)
    {
        return await new Directory().ServiceUrls(service);
    }

    /* XXX This interface is deprecated. Services may have multiple
     * URLs, and we cannot do liveness testing here as we don't know all
     * the protocols. */
    public async Task<string?> ServiceUrl(ServiceTypes service)
    {
        var urls = await ServiceUrls(service);
        return urls.Length > 0 ? urls[0] : null;
    }

    private async Task<string?[]> ServiceUrls(ServiceTypes service)
    {
        // TODO: Flesh out
        await FindServiceUrls(service.ToString());
        return null;
    }
    
    // TODO: More methods here
}
