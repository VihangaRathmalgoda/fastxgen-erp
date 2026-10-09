namespace FastXGen.Erp.Contracts.System;

public sealed record PingResponse(string Status, DateTimeOffset ServerTimeUtc);
