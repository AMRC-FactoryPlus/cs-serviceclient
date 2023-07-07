using System.Reflection.Emit;
using Newtonsoft.Json;

namespace utilities_dotnet;

public class ConfigDB : ServiceInterface
{
    public async Task<string> GetConfig(string app, string obj)
    {
        return "";
    }

    public async Task PutConfig(string app, string obj, string json)
    {
        
    }

    public async Task DeleteConfig(string app, string obj)
    {
        
    }

    public async Task PutConfig(string app, string obj, string type, string patch)
    {
        if (type != "merge") throw new Exception("Only merge-patch supported");

    }

    public async Task<Guid> CreateObject(string klass, Guid objUUID, bool exclusive)
    {
        return Guid.Empty;
    }

    public async Task DeleteObject(Guid objUUID)
    {
        
    }

    public async Task<Guid[]?> Search(string app, Dictionary<string, object> query, Dictionary<string, string> results, string? klass)
    {
        var qs = query
                      .Select(q => new KeyValuePair<string, string>(q.Key, JsonConvert.SerializeObject(q.Value)))
                      .ToDictionary(pair => pair.Key, pair => pair.Value);
        var localResults = results
                           .Select(q => new KeyValuePair<string, string>($"@{q.Key}", q.Value))
                           .ToDictionary(pair => pair.Key, pair => pair.Value);
        var queries = qs
                      .Concat(localResults)
                      .ToDictionary(pair => pair.Key, pair => pair.Value);

        var fetch = new FetchClass();
        var response = await fetch.Fetch($"/v1/app/{app}{klass}/search", "GET", queries, ServiceTypes.ConfigDB);

        if (response.Status != 200)
        {
            Console.WriteLine($"ConfigDB - Search failed: {response.Status}");

            return null;
        }

        var uuids = JsonConvert.DeserializeObject<Guid[]>(response.Content);
        return uuids;
    }

    public async Task<Guid> Resolve(string app, Dictionary<string, object> query, Dictionary<string, string> results, string? klass)
    {
        var uuids = await Search(app, query, new Dictionary<string, string>(), klass);

        if (uuids == null)
        {
            throw new Exception("Search didn't return an array");
        }

        if (uuids.Length == 0)
        {
            throw new Exception($"Search didn't return any results: {app} with {query}");
        }

        if (uuids.Length > 1)
        {
            throw new Exception($"Search return more than one result: {app} with {query}");
        }
        
        return uuids[0];
    }
}
