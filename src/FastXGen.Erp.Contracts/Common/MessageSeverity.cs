namespace FastXGen.Erp.Contracts.Common;

/// <summary>
/// How the client should present a message: Info and Success as a toast, Warning as a banner,
/// Error as a blocking alert. Stored in the database as TINYINT, so values must never change.
/// </summary>
public enum MessageSeverity : byte
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
}
