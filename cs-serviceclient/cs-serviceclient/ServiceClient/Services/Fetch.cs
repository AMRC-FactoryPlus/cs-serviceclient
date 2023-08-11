using System.Diagnostics;
using System.Security.Authentication;
using Cysharp.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace AMRC.FactoryPlus.ServiceClient.Services;

public struct TokenStruct
{
    public string Token;
    public string Expiry;

    public TokenStruct(string token, string expiry)
    {
        Token = token;
        Expiry = expiry;
    }
}

/// <summary>
/// The Fetch service interface
/// </summary>
public class FetchClass : ServiceInterface
{
    private readonly Dictionary<string, UniTask<FetchResponse>> _inflight;
    private readonly Dictionary<string, UniTask<string>> _inflightTokens;
    private readonly Dictionary<string, string> _tokens;
    
    /// <inheritdoc />
    public FetchClass(ServiceClient serviceClient) : base(serviceClient)
    {
        _inflight = new Dictionary<string, UniTask<FetchResponse>>();
        _inflightTokens = new Dictionary<string, UniTask<string>>();
        _tokens = new Dictionary<string, string>();
    }

    /// <inheritdoc />
    public override async UniTask<FetchResponse> Fetch(string url, string method = "GET", object? query = null, Guid? service = null, string? body = null, Dictionary<string, string>? headers = null, string? accept = null, string? contentType = null)
    {
        var serviceUrl = "";
        var amendedUrl = url;
        if (service != null)
        {
            serviceUrl = await ServiceClient.Discovery.ServiceUrl((Guid)service) ?? "";
            amendedUrl = serviceUrl.AppendPathSegment(url);
        }

        // Don't mess with stateful requests
        if (!IsIdempotent(method, headers, body))
        {
            return await DoFetch(amendedUrl, serviceUrl, method, query, service, body, headers, accept, contentType);
        }

        // If there is already a request to this URL, we can piggyback on it
        if (_inflight.TryGetValue(amendedUrl, out var currentInflight))
        {
            return await currentInflight;
        }

        var responseTask = DoFetch(amendedUrl, serviceUrl, method);
        // Store the task for other requests to use
        _inflight[amendedUrl] = responseTask;

        FetchResponse response;
        try
        {
            response = await responseTask;
        }
        finally
        {
            // Make sure to clear the inflight task whether it is a success or failure
             _inflight.Remove(amendedUrl);
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
                                    .SendAsync(new HttpMethod(method), new StringContent(body ?? ""), CancellationToken.None)
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
        if (!_tokens.TryGetValue(serviceUrl, out var token))
        {
            if (_inflightTokens.TryGetValue(serviceUrl, out var inflightToken))
            {
                var resolvedToken = await inflightToken;
                Debug.WriteLine($"Using token {resolvedToken} for {serviceUrl}");
                return resolvedToken;
            }
        }

        var isBad = !String.IsNullOrEmpty(badToken) && token == badToken;
        if (!String.IsNullOrEmpty(token) && !isBad)
        {
            return token;
        }

        var tokenRequest = FetchToken(serviceUrl);
        _inflightTokens[serviceUrl] = tokenRequest;
        try
        {
            token = await tokenRequest;
            Debug.WriteLine($"Using token {token} for {serviceUrl}");
        }
        finally
        {
            _inflightTokens.Remove(serviceUrl);
        }

        return token;
    }

    private async UniTask<string> FetchToken(string serviceUrl)
    {
        var tokenUrl = serviceUrl.AppendPathSegment("token");
        var res = await GssFetch(tokenUrl);

        if (res.Status != 200)
        {
            throw new Exception($"Token fetch failed for {serviceUrl}");
        }

        var token = JsonConvert.DeserializeObject<TokenStruct>(res.Content);
        _tokens[serviceUrl] = token.Token;
        return token.Token;
    }

    private async UniTask<FetchResponse> GssFetch(string tokenUrl)
    {
        if (!String.IsNullOrWhiteSpace(ServiceClient.ServiceUsername) && !String.IsNullOrWhiteSpace(ServiceClient.ServicePassword))
        {
            // Use basic auth
            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{ServiceClient.ServiceUsername}:{ServiceClient.ServicePassword}");
            var authString = Convert.ToBase64String(authBytes);
            var headers = AddAuthHeaders(null, "Basic", authString);
            try {
                var response = await tokenUrl.WithHeaders(headers).PostAsync(null, CancellationToken.None).WaitAsync(CancellationToken.None);
                return new FetchResponse(response.StatusCode, await response.GetStringAsync());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw new AuthenticationException("Unable to authenticate with Basic auth, are credentials correct?", e);
            }
        }

        throw new Exception("Only Basic auth supported at this time. Ensure config has username and password");
    }

    private Dictionary<string, string> AddAuthHeaders(Dictionary<string, string>? existingHeaders, string scheme,
                                                      string credentials)
    {
        var localHeaders = existingHeaders ?? new Dictionary<string, string>();
        localHeaders["Authorization"] = $"{scheme} {credentials}";
        return localHeaders;
    }

    private static bool IsIdempotent(string method, Dictionary<string, string>? headers, string? body)
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
