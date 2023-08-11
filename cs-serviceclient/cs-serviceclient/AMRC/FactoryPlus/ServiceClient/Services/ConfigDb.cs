using System.Diagnostics;
using AMRC.FactoryPlus.ServiceClient.Constants;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace AMRC.FactoryPlus.ServiceClient.Services;

/// <summary>
/// The body to be used when PUTing a config
/// </summary>
public struct PutConfigBody
{
    [JsonProperty("name")]
    public string Name;
    [JsonProperty("deleted")]
    public bool? Deleted;

    public PutConfigBody(string name, bool? deleted = null)
    {
        Name = name;
        Deleted = deleted;
    }
}

public struct ObjectRegistration
{
    public Guid Uuid;
    public Guid Class;

    public ObjectRegistration(Guid uuid, Guid @class)
    {
        Uuid = uuid;
        Class = @class;
    }
}

public struct PrinicpalConfig
{
    public string GroupId;
    public string NodeId;

    public PrinicpalConfig(string groupId, string nodeId)
    {
        GroupId = groupId;
        NodeId = nodeId;
    }
}

/// <summary>
/// The ConfigDB service interface
/// </summary>
public class ConfigDb : ServiceInterface
{
    /// <inheritdoc />
    public ConfigDb(ServiceClient serviceClient) : base(serviceClient)
    {
        ServiceType = ServiceTypes.ConfigDB;
    }
    
    public async UniTask<PrinicpalConfig?> GetConfig(Guid app, Guid obj)
    {
        var res = await Fetch($"/v1/app/{app}/object/{obj}");

        if (res.Status == 404)
        {
            return null;
        }

        if (res.Status != 200)
        {
            throw new Exception($"{res.Status}: Can't get {app} for {obj}");
        }
        
        return JsonConvert.DeserializeObject<PrinicpalConfig>(res.Content);
    }

    public async UniTask PutConfig(Guid app, Guid obj, string json)
    {
        var res = await Fetch($"/v1/app/{app}/object/{obj}", "PUT", null, null, json);
        if (res.Status == 204)
        {
            return;
        }

        throw new Exception($"{res.Status}: Can't set {app} for {obj}");
    }

    public async UniTask DeleteConfig(Guid app, Guid obj)
    {
        var res = await Fetch($"/v1/app/{app}/object/{obj}", "DELETE");
        if (res.Status == 204)
        {
            return;
        }

        throw new Exception($"{res.Status}: Can't remove {app} for {obj}");
    }

    public async UniTask PatchConfig(Guid app, Guid obj, string type, string patch)
    {
        if (type != "merge") throw new Exception("Only merge-patch supported");
        
        var res = await Fetch($"/v1/app/{app}/object/{obj}", "PATCH", null, null, patch, null, null, "application/merge-patch+json");
        if (res.Status == 204)
        {
            return;
        }

        throw new Exception($"{res.Status}: Can't patch {app} for {obj}");
    }

    public async UniTask<Guid> CreateObject(Guid klass, Guid? objUuidNullable = null, bool exclusive = false)
    {
        Guid objUuid = objUuidNullable ?? Guid.Empty;
        var res = await Fetch("/v1/object", "POST", null, null, JsonConvert.SerializeObject(new ObjectRegistration(objUuid, klass)));
        if (res.Status == 200 && exclusive)
        {
            throw new Exception($"Exclusive create of {objUuidNullable} failed");
        }

        if (res.Status == 201 || res.Status == 200)
        {
            return JsonConvert.DeserializeObject<ObjectRegistration>(res.Content).Uuid;
        }

        if (objUuidNullable != null)
        {
            throw new Exception($"{res.Status}: Creating {objUuidNullable} failed");
        }

        throw new Exception($"{res.Status}: Creating new {klass} failed");
    }

    public async UniTask DeleteObject(Guid objUuid)
    {
        var res = await Fetch($"/v1/object/{objUuid}", "DELETE");
        
        if (res.Status == 204)
        {
            return;
        }

        throw new Exception($"{res.Status}: Deleting {objUuid} failed");
    }

    public async UniTask<Guid[]?> Search(Guid app, Dictionary<string, object> query, Dictionary<string, string> results, string? klass = "")
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

        var response = await ServiceClient.Fetch.Fetch($"/v1/app/{app}{klass ?? ""}/search", "GET", queries, UUIDs.Service[ServiceTypes.ConfigDB]);

        if (response.Status != 200)
        {
            Debug.WriteLine($"ConfigDB - Search failed: {response.Status}");

            return null;
        }

        var uuids = JsonConvert.DeserializeObject<Guid[]>(response.Content);
        return uuids;
    }

    public async UniTask<Guid> Resolve(Guid app, Dictionary<string, object> query, Dictionary<string, string> results, string? klass)
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
