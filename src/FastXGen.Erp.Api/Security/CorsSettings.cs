namespace FastXGen.Erp.Api.Security;

public sealed class CorsSettings
{
    public const string SectionName = "Cors";

    /// <summary>Exact origins of the web apps allowed to call the API. Empty means no cross-origin calls.</summary>
    public string[] AllowedOrigins { get; set; } = [];
}
