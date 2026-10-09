namespace FastXGen.Erp.Contracts.Common;

/// <summary>
/// A message for the user that does not stop the request, for example
/// "stock for SKU-1001 is now below the reorder level". The client translates by <see cref="Code"/>.
/// </summary>
public sealed record UserMessage(string Code, string Text, MessageSeverity Severity);
