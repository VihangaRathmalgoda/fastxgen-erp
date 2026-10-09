namespace FastXGen.Erp.Application.Common.Exceptions;

/// <summary>The resource does not exist, or is not visible to the caller's tenant or scope.</summary>
public sealed class NotFoundException(string errorCode, string message) : AppException(errorCode, message);
