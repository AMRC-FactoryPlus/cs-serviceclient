using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace AMRC.FactoryPlus.ServiceClient;

public struct ServiceProvider
{
    public Guid Device;
    public string Url;

    public ServiceProvider(Guid device, string url)
    {
        Device = device;
        Url = url;
    }
}

public struct ServiceProviderList
{
    public List<ServiceProvider> List;

    public ServiceProviderList(List<ServiceProvider> list) => List = list;
}

/// <summary>
/// The Directory service interface
/// </summary>
public class Directory : ServiceInterface
{
    /// <inheritdoc />
    public Directory(ServiceClient serviceClient) : base(serviceClient)
    {
        ServiceType = ServiceTypes.Directory;
    }
    
    /// <summary>
    /// Gets a list of URLs that point to a service
    /// </summary>
    /// <param name="service">The service to query</param>
    /// <returns>List of URLs</returns>
    public async UniTask<string[]> ServiceUrls(string service)
    {
        var res = await Fetch($"/v1/service/{service}");

        if (res.Status == 404)
        {
            Console.WriteLine($"{res.Status}: Can't find service {service}");
            return new string[]{};
        }

        if (res.Status != 200)
        {
            throw new Exception($"{res.Status}: Can't get service records for {service}");
        }

        var specs = JsonConvert.DeserializeObject<ServiceProviderList>(res.Content);
        return specs.List.Select(s => s.Url).Where(s => !String.IsNullOrEmpty(s)).ToArray();
    }

    public async void RegisterServiceUrl(string service, string url)
    {
        var res = await Fetch($"/v1/service/{service}/advertisment", "PUT", null, null, $"{{\"url\": \"{url}\"}}");
        
        if (res.Status != 204)
        {
            throw new Exception($"{res.Status}: Can't register service {service}");
        }

        Console.WriteLine($"Registered {url} for {service}");
    }
}
