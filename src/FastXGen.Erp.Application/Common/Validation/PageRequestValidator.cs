using FastXGen.Erp.Contracts.Common;
using FluentValidation;

namespace FastXGen.Erp.Application.Common.Validation;

internal sealed class PageRequestValidator : AbstractValidator<PageRequest>
{
    public PageRequestValidator()
    {
        RuleFor(request => request.Page).GreaterThanOrEqualTo(1);
        RuleFor(request => request.PageSize).InclusiveBetween(1, ValidationRules.MaxPageSize);
    }
}
