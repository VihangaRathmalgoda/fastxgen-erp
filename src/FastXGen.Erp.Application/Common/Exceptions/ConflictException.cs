namespace FastXGen.Erp.Application.Common.Exceptions;

/// <summary>The request conflicts with current state, for example a stale RowVersion or a duplicate key.</summary>
public sealed class ConflictException(string errorCode, string message) : AppException(errorCode, message);
