namespace FastXGen.Erp.Application.Abstractions.Tenancy;

/// <summary>
/// The tenant and user of the current request.
/// Always resolved from the authenticated identity; never from the request body, header or query string.
/// </summary>
public interface ITenantContext
{
    int? TenantId { get; }

    Guid? UserId { get; }
}
