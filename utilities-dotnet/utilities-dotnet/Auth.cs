using Newtonsoft.Json;

namespace AMRC.FactoryPlus.ServiceClient;

public struct PostPrincipalBody
{
    public Guid Uuid;
    public string Kerberos;
    
    public PostPrincipalBody(Guid uuid, string kerberos)
    {
        this.Uuid = uuid;
        this.Kerberos = kerberos;
    }
}

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



public enum AceAction
{
    add,
    delete
}

public class Auth : ServiceInterface
{
    public Auth(ServiceClient serviceClient) : base(serviceClient)
    {
        _serviceType = ServiceTypes.Authentication;
    }

    public async Task<bool> CheckAcl(string principal, string permission, string target, bool wild)
    {
        var aclList = await FetchAcl(principal, _serviceClient.PermissionGroup);
        return aclList(permission, target, wild);
    }

    public async Task<Func<string, string, bool, bool>> FetchAcl(string principal, string group)
    {
        // TODO: Complete method
        return (s, s1, arg3) => true;
    }

    public async Task<Guid> ResolvePrincipal(string query)
    {
        // TODO: Complete method
        return Guid.Empty;
    }

    public async Task<Guid> FindPrincipal(string kind, string identifier)
    {
        // TODO: Complete method
        var uuid = String.IsNullOrEmpty(kind) ? await ResolvePrincipal("")
            : kind == "uuid" ? new Guid(identifier)
            : kind == "kerberos" ? await ResolvePrincipal("{kerberos: identifier}")
            : kind == "sparkplug" ? (await ResolvePrincipalByAddress(identifier))[0]
            : Guid.Empty;

        
        // TODO: Fetch

        var sp = await _serviceClient.ConfigDb.GetConfig(UUIDs.App[AppSubcomponents.SparkplugAddress], uuid.ToString());
        
        return Guid.Empty;
    }

    public async Task AddPrincipal(Guid uuid, string kerberos)
    {
        var res = await Fetch("authz/principal", "POST", null, null,
            JsonConvert.SerializeObject(new PostPrincipalBody(uuid, kerberos)));

        if (res.Status != 204)
        {
            throw new Exception($"{res.Status}: Can't create principal {kerberos}");
        }
    }

    public async Task<Guid> CreatePrincipal(string klass, string kerberos, string name)
    {
        var cdb = _serviceClient.ConfigDb;
        var uuid = await cdb.CreateObject(klass);

        try
        {
            await AddPrincipal(uuid, kerberos);
        }
        catch (Exception e)
        {
            await cdb.PutConfig(UUIDs.App[AppSubcomponents.Info], uuid.ToString(), JsonConvert.SerializeObject(new PutConfigBody(name, true)));
            throw;
        }

        if (!String.IsNullOrEmpty(name))
        {
            await cdb.PutConfig(UUIDs.App[AppSubcomponents.Info], uuid.ToString(),
                JsonConvert.SerializeObject(new PutConfigBody(name)));
        }

        return uuid;
    }

    public async Task AddAce(Guid principal, Guid permission, Guid target)
    {
        await EditAce(new PostAceBody(permission, target, AceAction.add, principal));
    }

    public async Task DeleteAce(Guid principal, Guid permission, Guid target)
    {
        await EditAce(new PostAceBody(permission, target, AceAction.delete, principal));
    }

    public async Task AddToGroup(Guid group, Guid member)
    {
        var res = await Fetch($"authz/group/{group}/{member}", "PUT");
        
        if (res.Status != 204)
        {
            throw new Exception($"{res.Status}: Can't add {member} to group {group}");
        }
    }

    public async Task RemoveFromGroup(Guid group, Guid member)
    {
        var res = await Fetch($"authz/group/{group}/{member}", "DELETE");
        
        if (res.Status != 204)
        {
            throw new Exception($"{res.Status}: Can't remove {member} from group {group}");
        }
    }

    private async Task<Guid[]> ResolvePrincipalByAddress(string address)
    {
        // TODO: Complete method
        var cdb = _serviceClient.ConfigDb;
        var ping = await cdb.Ping();
        // TODO: Check semver
        if (String.IsNullOrEmpty(ping))
        {
            return new[] { Guid.Empty };
        }
        
        return new[] { Guid.Empty };
    }

    private async Task EditAce(PostAceBody spec)
    {
        var res = await Fetch("authz/ace", "POST", null, null, JsonConvert.SerializeObject(spec));

        if (res.Status != 204)
        {
            throw new Exception($"{res.Status}: Can't {spec.Action} ACE");
        }
    }
}
