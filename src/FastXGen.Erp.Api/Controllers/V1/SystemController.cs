using FastXGen.Erp.Application.Abstractions.Time;
using FastXGen.Erp.Contracts.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastXGen.Erp.Api.Controllers.V1;

[ApiController]
[Route(ApiRoutes.V1 + "/system")]
public sealed class SystemController(IClock clock) : ControllerBase
{
    /// <summary>Confirms the API is reachable. Does not touch the database.</summary>
    [HttpGet("ping")]
    [AllowAnonymous]
    public ActionResult<PingResponse> Ping() => new PingResponse("ok", clock.UtcNow);
}
