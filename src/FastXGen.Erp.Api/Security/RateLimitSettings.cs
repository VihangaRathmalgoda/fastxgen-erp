namespace FastXGen.Erp.Api.Security;

public sealed class RateLimitSettings
{
    public const string SectionName = "RateLimiting";

    /// <summary>Requests per minute per signed-in user, or per IP address for anonymous calls.</summary>
    public int PermitsPerMinute { get; set; } = 300;
}
