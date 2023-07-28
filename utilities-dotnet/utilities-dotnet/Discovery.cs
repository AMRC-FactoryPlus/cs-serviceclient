using Cysharp.Threading.Tasks;

namespace AMRC.FactoryPlus.ServiceClient;

/// <summary>
/// The Discover service interface
/// </summary>
public class Discovery : ServiceInterface
{
    /// <inheritdoc />
    public Discovery(ServiceClient serviceClient) : base(serviceClient)
    {
        
    }

    /// <summary>
    /// Gets a list of URLs that point to a service
    /// </summary>
    /// <param name="service">The service to query</param>
    /// <returns>List of URLs</returns>
    public async UniTask<string[]> FindServiceUrls(string service)
    {
        return await _serviceClient.Directory.ServiceUrls(service);
    }

    /* XXX This interface is deprecated. Services may have multiple
     * URLs, and we cannot do liveness testing here as we don't know all
     * the protocols. */
    /// <summary>
    /// Gets the first known URL that points to a service
    /// </summary>
    /// <param name="service">The service to query</param>
    /// <returns>The URL</returns>
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
