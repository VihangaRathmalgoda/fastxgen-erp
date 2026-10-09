using FastXGen.Erp.Application.Abstractions.Time;

namespace FastXGen.Erp.Infrastructure.Time;

internal sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
