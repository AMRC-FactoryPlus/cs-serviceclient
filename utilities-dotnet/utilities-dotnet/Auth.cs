using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Semver;

namespace AMRC.FactoryPlus.ServiceClient;

public struct PostAceBody
{
    public Guid Permission;
    public Guid Target;
    public AceAction Action;
    public Guid Principal;
    public string Kerberos;

    public PostAceBody(Guid permission, Guid target, AceAction action, Guid? principal = null, string kerberos = "")
    {
        Permission = permission;
        Target = target;
        Action = action;
        Principal = principal ?? Guid.Empty;
        Kerberos = kerberos;
    }
}

public struct Address
{
    public string Group;
    public string Node;

    public Address(string group, string node)
    {
        Group = group;
        Node = node;
    }
}

public struct PrincipalMapping
{
    public Guid Uuid;
    public string Kerberos;
    public Address? Sparkplug;

    public PrincipalMapping(Guid uuid, string kerberos, Address? sparkplugAddress)
    {
        Uuid = uuid;
        Kerberos = kerberos;
        Sparkplug = sparkplugAddress;
    }
}

public struct FetchAclQuery
{
    public string Principal;
    public string Permission;
    [JsonProperty("by-uuid")]
    public bool ByUuid;

    public FetchAclQuery(string principal, string permission, bool byUuid)
    {
        Principal = principal;
        Permission = permission;
        ByUuid = byUuid;
    }
}

public struct Ace
{
    public Guid Permission;
    public Guid Target;
    public Guid Principal;
    public string Kerberos;

    public Ace(Guid permission, Guid target, Guid? principal = null, string kerberos = "")
    {
        Permission = permission;
        Target = target;
        Principal = principal ?? Guid.Empty;
        Kerberos = kerberos;
    }
}

public struct Acl
{
    [JsonProperty("acl")]
    public List<Ace> AclList;

    public Acl(List<Ace> aclList) => AclList = aclList;
}

public enum AceAction
{
    add,
    delete
}

/// <summary>
/// The Auth service interface
/// </summary>
public class Auth : ServiceInterface
{
    /// <inheritdoc />
    public Auth(ServiceClient serviceClient) : base(serviceClient)
    {
        ServiceType = ServiceTypes.Authentication;
    }

    public async UniTask<bool> CheckAcl(string? kerberos, Guid? uuid, string permission, string target, bool wild)
    {
        var aclList = await FetchAcl(kerberos, uuid, ServiceClient.PermissionGroup ?? "");
        return aclList(permission, target, wild);
    }

    public async UniTask<Func<string, string, bool, bool>> FetchAcl(string? kerberos, Guid? uuid, string permissionGroup)
    {
        var type = "kerberos";
        var principal = "";
        if (!string.IsNullOrEmpty(kerberos))
        {
            principal = kerberos;
        }
        else if (uuid != Guid.Empty)
        {
            type = "uuid";
            principal = uuid.ToString();
        }
        else
        {
            type = "";
        }

        if (String.IsNullOrEmpty(type))
        {
            Console.WriteLine($"Unrecognised principal request: {kerberos}, {uuid}");
            return (s1, s2, b1) => false;
        }

        if (!String.IsNullOrEmpty(ServiceClient.RootPrincipal)
         && type == "kerberos"
         && principal == ServiceClient.RootPrincipal)
        {
            return (s1, s2, b1) => true;
        }

        var isUuid = type == "uuid";

        var res = await ServiceClient.Fetch.Fetch(
            "/authz/acl",
            "GET",
            new FetchAclQuery(principal, permissionGroup, isUuid),
            UUIDs.Service[ServiceTypes.Authentication]);

        if (res.Status != 200)
        {
            Console.WriteLine($"{res.Status}: Failed to read ACL for {principal}");
            return (s1, s2, b1) => false;
        }

        var acl = JsonConvert.DeserializeObject<Acl>(res.Content);

        return (permission, target, wild) =>
            acl.AclList.Any(ace =>
                ace.Permission == new Guid(permission)
                && (ace.Target == new Guid(target)
                || (wild && ace.Target == Guid.Empty))
            );
    }

    public async UniTask<Guid> ResolvePrincipal(string query)
    {
        var res = await ServiceClient.Fetch.Fetch(
            "/authz/principal/find",
            "GET", 
            query, 
            UUIDs.Service[ServiceTypes.Authentication]
            );

        if (res.Status != 200)
        {
            Console.WriteLine($"{res.Status}: Failed to resolve {query}");
            return Guid.Empty;
        }

        var uuid = new Guid(res.Content);
        Console.WriteLine($"{res.Status}: Resolved {query} to {uuid}");
        return uuid;
    }

    public async UniTask<PrincipalMapping?> FindPrincipal(Guid? guid = null, string? kerberos = null, Address? address = null)
    {
        var uuid = guid != null && guid != Guid.Empty ? guid
            : !String.IsNullOrEmpty(kerberos) ? await ResolvePrincipal($"{{kerberos: {kerberos}}}")
            : address != null ? (await ResolvePrincipalByAddress(address ?? default))[0]
            : await ResolvePrincipal("");

        if (uuid == Guid.Empty)
        {
            return null;
        }

        var res = await Fetch($"/authz/principal/{uuid}");

        if (res.Status != 200)
        {
            Console.WriteLine($"{res.Status}: Failed to fetch principal {uuid}");
            return null;
        }

        var ids = JsonConvert.DeserializeObject<PrincipalMapping>(res.Content);
        
        var spConfig = await ServiceClient.ConfigDb.GetConfig(UUIDs.App[AppSubcomponents.SparkplugAddress], (Guid)uuid);
        if (spConfig != null)
        {
            ids.Sparkplug = new Address(spConfig?.GroupId, spConfig?.NodeId);
        }
        
        return ids;
    }

    public async UniTask AddPrincipal(Guid uuid, string kerberos)
    {
        var res = await Fetch(
            "authz/principal", 
            "POST", 
            null, 
            null,
            JsonConvert.SerializeObject(new PrincipalMapping(uuid, kerberos, null))
            );

        if (res.Status != 204)
        {
            throw new Exception($"{res.Status}: Can't create principal {kerberos}");
        }
    }

    public async UniTask<Guid> CreatePrincipal(Guid klass, string kerberos, string name)
    {
        var cdb = ServiceClient.ConfigDb;
        var uuid = await cdb.CreateObject(klass);

        try
        {
            await AddPrincipal(uuid, kerberos);
        }
        catch (Exception e)
        {
            await cdb.PutConfig(UUIDs.App[AppSubcomponents.Info], uuid, JsonConvert.SerializeObject(new PutConfigBody(name, true)));
            throw;
        }

        if (!String.IsNullOrEmpty(name))
        {
            await cdb.PutConfig(UUIDs.App[AppSubcomponents.Info], uuid,
                JsonConvert.SerializeObject(new PutConfigBody(name)));
        }

        return uuid;
    }

    public async UniTask AddAce(Guid principal, Guid permission, Guid target)
    {
        await EditAce(new PostAceBody(permission, target, AceAction.add, principal));
    }

    public async UniTask DeleteAce(Guid principal, Guid permission, Guid target)
    {
        await EditAce(new PostAceBody(permission, target, AceAction.delete, principal));
    }

    public async UniTask AddToGroup(Guid group, Guid member)
    {
        var res = await Fetch($"authz/group/{group}/{member}", "PUT");
        
        if (res.Status != 204)
        {
            throw new Exception($"{res.Status}: Can't add {member} to group {group}");
        }
    }

    public async UniTask RemoveFromGroup(Guid group, Guid member)
    {
        var res = await Fetch($"authz/group/{group}/{member}", "DELETE");
        
        if (res.Status != 204)
        {
            throw new Exception($"{res.Status}: Can't remove {member} from group {group}");
        }
    }

    private async UniTask<Guid[]> ResolvePrincipalByAddress(Address address)
    {
        var cdb = ServiceClient.ConfigDb;
        var ping = await cdb.Ping();
        if (String.IsNullOrEmpty(ping?.Version) || !SemVersion.Parse(ping?.Version).Satisfies(">=1.7 || =1.7.0-bmz"))
        {
            Console.WriteLine($"ConfigDB is too old to search for addresses ({ping?.Version})");
            return new[] { Guid.Empty };
        }

        var searchQuery = new Dictionary<string, object>
            {
                ["group_id"] = address.Group, ["node_id"] = address.Node, ["device_id"] = ""
            };
        var uuids = await cdb.Search(UUIDs.App[AppSubcomponents.SparkplugAddress], searchQuery, new Dictionary<string, string>());
        if (uuids is {Length: 1})
        {
            return uuids;
        }

        if (uuids is {Length: > 1})
        {
            Console.WriteLine($"Multiple results resolving Sparkplug address {address}");
        }
        
        return new[] { Guid.Empty };
    }

    private async UniTask EditAce(PostAceBody spec)
    {
        var res = await Fetch("authz/ace", "POST", null, null, JsonConvert.SerializeObject(spec));

        if (res.Status != 204)
        {
            throw new Exception($"{res.Status}: Can't {spec.Action} ACE");
        }
    }
}
