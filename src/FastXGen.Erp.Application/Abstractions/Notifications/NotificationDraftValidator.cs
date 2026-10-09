using FastXGen.Erp.Application.Common.Validation;
using FluentValidation;

namespace FastXGen.Erp.Application.Abstractions.Notifications;

/// <summary>
/// Notifications are built by our own code, but their text often includes user-entered values
/// (product names, customer names), so they are validated like any other input.
/// </summary>
internal sealed class NotificationDraftValidator : AbstractValidator<NotificationDraft>
{
    public NotificationDraftValidator()
    {
        RuleFor(draft => draft.RecipientUserId).EntityId();
        RuleFor(draft => draft.Category).NotEmpty().Code(50);
        RuleFor(draft => draft.Severity).IsInEnum();
        RuleFor(draft => draft.Title).NotEmpty().PlainText(200);
        RuleFor(draft => draft.Body).PlainText(1000);
        RuleFor(draft => draft.Link).AppLink();
    }
}
