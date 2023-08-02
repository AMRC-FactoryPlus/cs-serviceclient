using Cysharp.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace AMRC.FactoryPlus.ServiceClient;

/// <summary>
/// The Fetch service interface
/// </summary>
public class FetchClass : ServiceInterface
{
    private Dictionary<string, UniTask<FetchResponse>> inflight;
    private Dictionary<string, UniTask<string>> inflightTokens;
    private Dictionary<string, string> tokens;
    
    /// <inheritdoc />
    public FetchClass(ServiceClient serviceClient) : base(serviceClient)
    {
        inflight = new Dictionary<string, UniTask<FetchResponse>>();
        inflightTokens = new Dictionary<string, UniTask<string>>();
        tokens = new Dictionary<string, string>();
    }

    /// <inheritdoc />
    public override async UniTask<FetchResponse> Fetch(string url, string method, object? query = null, Guid? service = null, string? body = null, Dictionary<string, string>? headers = null, string? accept = null, string? contentType = null)
    {
        var serviceUrl = "";
        var amendedUrl = url;
        if (service != null)
        {
            serviceUrl = await ServiceClient.Discovery.ServiceUrl((Guid)service) ?? "";
            amendedUrl = url.AppendPathSegment(serviceUrl);
        }

        // Don't mess with stateful requests
        if (!isIdempotent(method, headers, body))
        {
            return await DoFetch(amendedUrl, serviceUrl, method, query, service, body, headers, accept, contentType);
        }

        // If there is already a request to this URL, we can piggyback on it
        if (inflight.TryGetValue(amendedUrl, out var currentInflight))
        {
            return await currentInflight;
        }

        var responseTask = DoFetch(amendedUrl, serviceUrl, method);
        // Store the task for other requests to use
        inflight[amendedUrl] = responseTask;

        FetchResponse response;
        try
        {
            response = await responseTask;
        }
        finally
        {
            // Make sure to clear the inflight task whether it is a success or failure
             inflight.Remove(amendedUrl);
        }
        
        return response;
    }

    private async UniTask<FetchResponse> DoFetch(string url, string serviceUrl, string method, object? query = null, Guid? service = null, string? body = null, Dictionary<string, string>? headers = null, string? accept = null, string? contentType = null)
    {
        var token = "";

        async Task<FetchResponse> tryFetch()
        {
            token = await ServiceToken(serviceUrl, token);
            if (String.IsNullOrEmpty(token))
            {
                return new FetchResponse(401, "");
            }

            var localHeaders = new Dictionary<string, string>(headers ?? new Dictionary<string, string>()) {["Accept"] = accept ?? "application/json"};
            if (!String.IsNullOrWhiteSpace(body))
            {
                localHeaders["Content-Type"] = contentType ?? "application/json";
            }

            localHeaders = AddAuthHeaders(headers, "Bearer", token);

            var response = await url.WithHeaders(localHeaders)
                                    .SetQueryParams(query)
                                    .SendUrlEncodedAsync(new HttpMethod(method), body, CancellationToken.None)
                                    .WaitAsync(CancellationToken.None);

            return new FetchResponse(response.StatusCode, await response.GetStringAsync());
        }

        var res = await tryFetch();
        if (res.Status == 401)
        {
            res = await tryFetch();
        }
        
        return res;
    }

    private async UniTask<string> ServiceToken(string serviceUrl, string? badToken)
    {
        var token = "";
        if (tokens.TryGetValue(serviceUrl, out token))
        { }
        else
        {
            if (inflightTokens.TryGetValue(serviceUrl, out var inflightToken))
            {
                var resolvedToken = await inflightToken;
                Console.WriteLine($"Using token {resolvedToken} for {serviceUrl}");
                return resolvedToken;
            }
        }

        var isBad = !String.IsNullOrEmpty(badToken) && token == badToken;
        if (String.IsNullOrEmpty(token) || isBad)
        {
            var tokenRequest = FetchToken(serviceUrl);
            inflightTokens[serviceUrl] = tokenRequest;
        }

        try
        {
            token = await FetchToken(serviceUrl);
            Console.WriteLine($"Using token {token} for {serviceUrl}");
        }
        finally
        {
            inflightTokens.Remove(serviceUrl);
        }
        return token;
    }

    private async UniTask<string> FetchToken(string serviceUrl)
    {
        var tokenUrl = $"/token/{serviceUrl}";
        var res = await GssFetch(tokenUrl);

        if (res.Status != 200)
        {
            throw new Exception($"Token fetch failed for {serviceUrl}");
        }

        var token = res.Content;
        tokens[serviceUrl] = token;
        return token;
    }

    private async UniTask<FetchResponse> GssFetch(string tokenUrl)
    {
        if (!String.IsNullOrWhiteSpace(ServiceClient.ServiceUsername) && !String.IsNullOrWhiteSpace(ServiceClient.ServicePassword))
        {
            // Use basic auth
            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{ServiceClient.ServiceUsername}:{ServiceClient.ServicePassword}");
            var authString = System.Convert.ToBase64String(authBytes);
            var headers = AddAuthHeaders(null, "Basic", authString);
            var response = await tokenUrl.WithHeaders(headers).SendUrlEncodedAsync(new HttpMethod("POST"), null, CancellationToken.None).WaitAsync(CancellationToken.None);

            return new FetchResponse(response.StatusCode, await response.GetStringAsync());
        }
        else
        {
            throw new Exception("Only Basic auth supported at this time. Ensure config has username and password");
        }
        return new FetchResponse(400, "");
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
