namespace FastXGen.Erp.Application.Common.Exceptions;

/// <summary>The request is well formed but breaks a business rule.</summary>
public sealed class BusinessRuleException(string errorCode, string message) : AppException(errorCode, message);
