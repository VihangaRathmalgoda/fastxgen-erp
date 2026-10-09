namespace FastXGen.Erp.Application.Abstractions.Time;

/// <summary>
/// The only source of "now" in the application, so time-dependent rules can be tested.
/// </summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
