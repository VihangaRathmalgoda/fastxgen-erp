using FastXGen.Erp.Application.Common.Exceptions;
using FastXGen.Erp.Application.Common.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastXGen.Erp.IntegrationTests.Api.Probe;

/// <summary>Test-only endpoints that exercise the validation and error pipeline. Never part of the product.</summary>
[ApiController]
[Route("api/v1/test-probe")]
public sealed class ProbeController : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public ActionResult<ProbeRequest> Echo(ProbeRequest request) => request;

    [HttpGet("unvalidated")]
    [AllowAnonymous]
    public string Unvalidated([FromQuery] UnvalidatedProbeRequest request) => request.Value ?? string.Empty;

    [HttpGet("business-rule")]
    [AllowAnonymous]
    public IActionResult BusinessRule() =>
        throw new BusinessRuleException("sales.discount.limit_exceeded", "Discount is above your limit.");

    [HttpGet("internal-validation")]
    [AllowAnonymous]
    public IActionResult InternalValidation() =>
        throw new RequestValidationException(new Dictionary<string, string[]> { ["Lines[0].UnitPrice"] = ["Too high."] });
}

public sealed record ProbeRequest(string Name, string Code, decimal Amount);

public sealed record UnvalidatedProbeRequest(string? Value);

public sealed class ProbeRequestValidator : AbstractValidator<ProbeRequest>
{
    public ProbeRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().PlainText(50);
        RuleFor(request => request.Code).NotEmpty().Code(20);
        RuleFor(request => request.Amount).Money();
    }
}
