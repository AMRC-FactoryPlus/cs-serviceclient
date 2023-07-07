using Flurl;
using Flurl.Http;

namespace utilities_dotnet;

public class FetchClass : ServiceInterface
{
    public FetchClass() : base()
    {
        
    }

    public override async Task<FetchResponse> Fetch(string url, string method, object? query = null, ServiceTypes? service = null, string? body = null, Dictionary<string, string>? headers = null, string? accept = null, string? contentType = null)
    {
        string serviceUrl = "";
        if (service != null)
        {
            serviceUrl = await new Discovery().ServiceUrl((ServiceTypes)service) ?? "";
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

    private async Task<FetchResponse> DoFetch(string url, string method, object? query = null, ServiceTypes? service = null, string? body = null, Dictionary<string, string>? headers = null, string? accept = null, string? contentType = null)
    {
        
        var localHeaders = new Dictionary<string, string>(headers ?? new Dictionary<string, string>()) {["Accept"] = accept ?? "application/json"};
        if (!String.IsNullOrWhiteSpace(body))
        {
            localHeaders["Content-Type"] = contentType ?? "application/json";
        }

        var response = await url.WithHeaders(localHeaders).SetQueryParams(query).SendUrlEncodedAsync(new HttpMethod(method), body, CancellationToken.None).WaitAsync(CancellationToken.None);
        
        return new FetchResponse(response.StatusCode, await response.GetStringAsync());
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
