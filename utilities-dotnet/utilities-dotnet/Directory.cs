using Cysharp.Threading.Tasks;

namespace AMRC.FactoryPlus.ServiceClient;

/// <summary>
/// The Directory service interface
/// </summary>
public class Directory : ServiceInterface
{
    /// <inheritdoc />
    public Directory(ServiceClient serviceClient) : base(serviceClient)
    {
        _serviceType = ServiceTypes.Directory;
    }
    
    /// <summary>
    /// Gets a list of URLs that point to a service
    /// </summary>
    /// <param name="service">The service to query</param>
    /// <returns>List of URLs</returns>
    public async UniTask<string[]> ServiceUrls(string service)
    {
        // TODO: Fetch stuff
        return new[] { "" };
    }

    public void RegisterServiceUrl(string service, string url)
    {
        // TODO: PUT stuff
    }
}
