using Flurl.Http;

namespace AMRC.FactoryPlus.ServiceClient;

public struct FetchResponse
{
    public FetchResponse(int status, string content)
    {
        Status = status;
        Content = content;
    }
    
    public int Status { get; }
    public string Content { get; }
}

public class ServiceInterface
{
    internal ServiceClient _serviceClient;
    internal ServiceTypes _serviceType;
    
    public ServiceInterface(ServiceClient serviceClient)
    {
        _serviceClient = serviceClient;
    }

    public virtual async Task<FetchResponse> Fetch(string url, string method, object? query = null, ServiceTypes? service = null, string? body = null, Dictionary<string, string>? headers = null, string? accept = null, string? contentType = null)
    {
        var localHeaders = new Dictionary<string, string>(headers ?? new Dictionary<string, string>()) {["Accept"] = accept ?? "application/json"};
        if (!String.IsNullOrWhiteSpace(body))
        {
            localHeaders["Content-Type"] = contentType ?? "application/json";
        }

        var response = await url.WithHeaders(localHeaders).SetQueryParams(query).SendUrlEncodedAsync(new HttpMethod(method), body, CancellationToken.None).WaitAsync(CancellationToken.None);
        
        return new FetchResponse(response.StatusCode, await response.GetStringAsync());
    }

    public async Task<string> Ping()
    {
        var response = await Fetch("/ping", "GET");

        if (response.Status != 200)
        {
            return null;
        }

        return response.Content;
    }
}
