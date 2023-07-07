namespace utilities_dotnet;

public class Discovery : ServiceInterface
{
    public Discovery() : base()
    {
        
    }

    /* XXX This interface is deprecated. Services may have multiple
     * URLs, and we cannot do liveness testing here as we don't know all
     * the protocols. */
    public static string? ServiceUrl(ServiceTypes service)
    {
        var urls = ServiceUrls(service);
        return urls.Length > 0 ? urls[0] : null;
    }

    private static string?[] ServiceUrls(ServiceTypes service)
    {
        return null;
    }
}
