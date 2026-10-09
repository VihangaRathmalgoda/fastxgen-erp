using System.Globalization;
using FastXGen.Erp.Application.Abstractions.Tenancy;

namespace FastXGen.Erp.Api.Tenancy;

/// <summary>
/// Reads tenant and user from the authenticated principal's claims, and from nothing else.
/// An anonymous request has no tenant, so tenant-scoped procedures reject it.
/// </summary>
internal sealed class ClaimsTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    public const string TenantClaim = "tenant_id";
    public const string UserClaim = "sub";

    public int? TenantId => ReadClaim(TenantClaim, int.Parse);

    public Guid? UserId => ReadClaim(UserClaim, Guid.Parse);

    private T? ReadClaim<T>(string type, Func<string, IFormatProvider, T> parse)
        where T : struct
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var value = user.FindFirst(type)?.Value;
        return value is null ? null : parse(value, CultureInfo.InvariantCulture);
    }
}
