using Cysharp.Threading.Tasks;

namespace AMRC.FactoryPlus.ServiceClient;

/// <summary>
/// The Discover service interface
/// </summary>
public class Discovery : ServiceInterface
{
    private readonly Dictionary<Guid, string> _urls;
    private readonly Dictionary<Guid, string?> _presets;

    /// <inheritdoc />
    public Discovery(ServiceClient serviceClient) : base(serviceClient)
    {
        _urls = new Dictionary<Guid, string>();
        _presets = new Dictionary<Guid, string?>
        {
            {UUIDs.Service[ServiceTypes.Authentication], _serviceClient.AuthnUrl},
            {UUIDs.Service[ServiceTypes.ConfigDB], _serviceClient.ConfigDbUrl},
            {UUIDs.Service[ServiceTypes.Directory], _serviceClient.DirectoryUrl},
            {UUIDs.Service[ServiceTypes.MQTT], _serviceClient.MqttUrl}
        };

        foreach (var preset in _presets)
        {
            if (String.IsNullOrEmpty(preset.Value))
            {
                continue;
            }

            Console.WriteLine($"Preset URL for {preset.Key}: {preset.Value}");
            SetServiceUrl(preset.Key, preset.Value);
        }
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
    public async UniTask<string?> ServiceUrl(Guid service)
    {
        var urls = await ServiceUrls(service);
        return urls.Length > 0 ? urls[0] : null;
    }

    private async UniTask<string?[]> ServiceUrls(Guid service)
    {
        if (_urls.TryGetValue(service, out var url))
        {
            Console.WriteLine($"[{service}]Found {url} preconfigured");
            return new[] {url};
        }
        
        var urls = await FindServiceUrls(service.ToString());

        if (urls is {Length: > 0})
        {
            Console.WriteLine($"[{service}] Discover returned {String.Join(", ", urls)}");
            return urls;
        }
        
        return Array.Empty<string>();
    }

    private void SetServiceUrl(Guid service, string url)
    {
        _urls[service] = url;
    }
}
