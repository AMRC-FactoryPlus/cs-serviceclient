using System.Diagnostics;
using AMRC.FactoryPlus.ServiceClient.Constants;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace AMRC.FactoryPlus.ServiceClient.Services;

/// <summary>
/// The response expected from a Fetch request
/// </summary>
public struct FetchResponse
{
    /// <summary>
    /// Creates a FetchResponse object
    /// </summary>
    /// <param name="status">The status code of the request</param>
    /// <param name="content">The content of the response</param>
    public FetchResponse(int status, string content)
    {
        Status = status;
        Content = content;
    }
    
    /// <summary>
    /// The status code returned by the request
    /// </summary>
    public int Status { get; }
    /// <summary>
    /// The content returned in the response
    /// </summary>
    public string Content { get; }
}

public struct PingResponse
{
    public readonly string Version;

    public PingResponse(string version) => Version = version;
}

/// <summary>
/// A base class for the different services within the stack
/// </summary>
public class ServiceInterface
{
    /// <summary>
    /// A reference to the ServiceClient that has been created
    /// </summary>
    internal readonly ServiceClient ServiceClient;
    /// <summary>
    /// The ServiceType that this service is
    /// </summary>
    internal ServiceTypes ServiceType;

    private int _recursionCount = 0;
    
    /// <summary>
    /// Creates a ServiceInterface object
    /// </summary>
    /// <param name="serviceClient">The ServiceClient that this interface will talk to</param>
    public ServiceInterface(ServiceClient serviceClient)
    {
        ServiceClient = serviceClient;
    }

    /// <summary>
    /// Fetch a resource
    /// </summary>
    /// <param name="url">The URL to request</param>
    /// <param name="method">The method to use for the request</param>
    /// <param name="query">The query parameters to use</param>
    /// <param name="service">The service to be queried</param>
    /// <param name="body">The body of the request</param>
    /// <param name="headers">The headers of the request</param>
    /// <param name="accept">The format to accept back</param>
    /// <param name="contentType">The type of content being sent</param>
    /// <returns>A FetchResponse object</returns>
    public virtual async UniTask<FetchResponse> Fetch(string url, string method = "GET", object? query = null, Guid? service = null, string? body = null, Dictionary<string, string>? headers = null, string? accept = null, string? contentType = null)
    {
        var localHeaders = new Dictionary<string, string>(headers ?? new Dictionary<string, string>()) {["Accept"] = accept ?? "application/json"};
        if (!String.IsNullOrWhiteSpace(body))
        {
            localHeaders["Content-Type"] = contentType ?? "application/json";
        }
        
        Debug.Assert(!String.IsNullOrWhiteSpace(ServiceClient.DirectoryUrl), "The provided DirectoryUrl is empty, which is unlikely to work.");

        if (_recursionCount > 6)
        {
            throw new Exception("Fetch recursion detected, unable to lookup the required service URLs. Have you provided a working Directory URL?");
        }

        _recursionCount++;
        var res = await ServiceClient.Fetch.Fetch(url, method, query, UUIDs.Service[ServiceType], body, localHeaders, accept, contentType);
        _recursionCount = 0;
        return new FetchResponse(res.Status, res.Content);
    }

    /// <summary>
    /// Attempts to ping the stack
    /// </summary>
    /// <returns>A string of information</returns>
    public async UniTask<PingResponse?> Ping()
    {
        var response = await Fetch("/ping");

        if (response.Status != 200)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<PingResponse>(response.Content);
    }
}
