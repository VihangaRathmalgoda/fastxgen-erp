namespace FastXGen.Erp.Application.Common.Exceptions;

/// <summary>The caller is authenticated but not allowed to perform this action in this scope.</summary>
public sealed class ForbiddenException(string errorCode, string message) : AppException(errorCode, message);
