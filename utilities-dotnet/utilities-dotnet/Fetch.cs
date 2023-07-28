using Cysharp.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace AMRC.FactoryPlus.ServiceClient;

public class FetchClass : ServiceInterface
{
    public FetchClass(ServiceClient serviceClient) : base(serviceClient)
    {
        // TODO: Set up tokens and inflight requests
    }

    public override async UniTask<FetchResponse> Fetch(string url, string method, object? query = null, ServiceTypes? service = null, string? body = null, Dictionary<string, string>? headers = null, string? accept = null, string? contentType = null)
    {
        string serviceUrl = "";
        if (service != null)
        {
            serviceUrl = await _serviceClient.Discovery.ServiceUrl((ServiceTypes)service) ?? "";
            url = url.AppendPathSegment(serviceUrl);
        }
        
        var localHeaders = new Dictionary<string, string>(headers ?? new Dictionary<string, string>()) {["Accept"] = accept ?? "application/json"};
        if (!String.IsNullOrWhiteSpace(body))
        {
            localHeaders["Content-Type"] = contentType ?? "application/json";
        }

        var response = await DoFetch(url, method);
        
        // TODO: Complete method
        
        return response;
    }

    private async UniTask<FetchResponse> DoFetch(string url, string method, object? query = null, ServiceTypes? service = null, string? body = null, Dictionary<string, string>? headers = null, string? accept = null, string? contentType = null)
    {
        // TODO: Complete method
        var localHeaders = new Dictionary<string, string>(headers ?? new Dictionary<string, string>()) {["Accept"] = accept ?? "application/json"};
        if (!String.IsNullOrWhiteSpace(body))
        {
            localHeaders["Content-Type"] = contentType ?? "application/json";
        }

        var response = await url.WithHeaders(localHeaders).SetQueryParams(query).SendUrlEncodedAsync(new HttpMethod(method), body, CancellationToken.None).WaitAsync(CancellationToken.None);
        
        return new FetchResponse(response.StatusCode, await response.GetStringAsync());
    }

    private async UniTask<string> ServiceToken(string serviceUrl)
    {
        // TODO: Complete method
        var token = await FetchToken(serviceUrl);
        return token;
    }

    private async UniTask<string> FetchToken(string serviceUrl)
    {
        // TODO: Complete method
        var tokenUrl = serviceUrl;
        var token = await GssFetch(tokenUrl);
        return token;
    }

    private async UniTask<string> GssFetch(string tokenUrl)
    {
        // TODO: Complete method
        if (!String.IsNullOrWhiteSpace(_serviceClient.ServiceUsername) && !String.IsNullOrWhiteSpace(_serviceClient.ServicePassword))
        {
            // Use basic auth
            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{_serviceClient.ServiceUsername}:{_serviceClient.ServicePassword}");
            var authString = System.Convert.ToBase64String(authBytes);
            var headers = AddAuthHeaders(null, "Basic", authString);
            var response = await tokenUrl.WithHeaders(headers).SendUrlEncodedAsync(new HttpMethod("POST"), null, CancellationToken.None).WaitAsync(CancellationToken.None);

            return await response.GetStringAsync();
        }
        return "";
    }

    private Dictionary<string, string> AddAuthHeaders(Dictionary<string, string>? existingHeaders, string scheme,
                                                      string credentials)
    {
        var localHeaders = existingHeaders ?? new Dictionary<string, string>();
        localHeaders["Authorization"] = $"{scheme} {credentials}";
        return localHeaders;
    }

    private bool isIdempotent(string method, Dictionary<string, string>? headers, string? body)
    {
        // Only GET requests can be idempotent
        if (method != "GET")
        {
            return false;
        }

        // Can't be idempotent with headers
        if (headers is {Count: > 0})
        {
            return false;
        }

        // Can only be idempotent with an empty body
        return String.IsNullOrWhiteSpace(body);
    }
}
