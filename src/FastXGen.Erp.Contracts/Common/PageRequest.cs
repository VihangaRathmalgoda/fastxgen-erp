namespace FastXGen.Erp.Contracts.Common;

/// <summary>Paging input shared by every list endpoint. Page size is capped at 200.</summary>
public sealed record PageRequest(int Page = 1, int PageSize = 50);
