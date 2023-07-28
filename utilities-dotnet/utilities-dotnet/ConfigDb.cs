﻿using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace AMRC.FactoryPlus.ServiceClient;

public struct PutConfigBody
{
    public string name;
    public bool? deleted;

    public PutConfigBody(string name, bool? deleted = null)
    {
        this.name = name;
        this.deleted = deleted;
    }
}

public class ConfigDb : ServiceInterface
{
    public ConfigDb(ServiceClient serviceClient) : base(serviceClient)
    {
        _serviceType = ServiceTypes.ConfigDB;
    }
    
    public async UniTask<string> GetConfig(string app, string obj)
    {
        // TODO: Complete method
        return "";
    }

    public async UniTask PutConfig(string app, string obj, string json)
    {
        // TODO: Complete method
    }

    public async UniTask DeleteConfig(string app, string obj)
    {
        // TODO: Complete method
    }

    public async UniTask PatchConfig(string app, string obj, string type, string patch)
    {
        if (type != "merge") throw new Exception("Only merge-patch supported");

        // TODO: Complete method
    }

    public async UniTask<Guid> CreateObject(string klass, Guid? objUUIDNullable = null, bool exclusive = false)
    {
        Guid objUUID = objUUIDNullable ?? Guid.Empty;
        // TODO: Complete method
        return Guid.Empty;
    }

    public async UniTask DeleteObject(Guid objUUID)
    {
        // TODO: Complete method
    }

    public async UniTask<Guid[]?> Search(string app, Dictionary<string, object> query, Dictionary<string, string> results, string? klass)
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

        var response = await _serviceClient.Fetch.Fetch($"/v1/app/{app}{klass}/search", "GET", queries, ServiceTypes.ConfigDB);

        if (response.Status != 200)
        {
            Console.WriteLine($"ConfigDB - Search failed: {response.Status}");

            return null;
        }

        var uuids = JsonConvert.DeserializeObject<Guid[]>(response.Content);
        return uuids;
    }

    public async UniTask<Guid> Resolve(string app, Dictionary<string, object> query, Dictionary<string, string> results, string? klass)
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