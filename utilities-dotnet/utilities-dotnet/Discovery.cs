using Cysharp.Threading.Tasks;

namespace AMRC.FactoryPlus.ServiceClient;

public class Discovery : ServiceInterface
{
    public Discovery(ServiceClient serviceClient) : base(serviceClient)
    {
        
    }

    public async UniTask<string[]> FindServiceUrls(string service)
    {
        return await _serviceClient.Directory.ServiceUrls(service);
    }

    /* XXX This interface is deprecated. Services may have multiple
     * URLs, and we cannot do liveness testing here as we don't know all
     * the protocols. */
    public async UniTask<string?> ServiceUrl(ServiceTypes service)
    {
        var urls = await ServiceUrls(service);
        return urls.Length > 0 ? urls[0] : null;
    }

    private async UniTask<string?[]> ServiceUrls(ServiceTypes service)
    {
        // TODO: Flesh out
        await FindServiceUrls(service.ToString());
        return null;
    }
    
    // TODO: More methods here
}
