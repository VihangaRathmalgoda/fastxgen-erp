# 0013. User messages, alerts and notifications

- Status: Accepted
- Date: 2026-10-09

## Context

Users need clear feedback: field errors on forms, alerts when an action is refused, warnings when an action
succeeds with a caveat, and notifications about things that happen later (an approval waiting, low stock).
If each module invents its own format, the frontend cannot handle them consistently or translate them.

## Decision

Four kinds of message, each with one shape:

| Kind | When | Shape | Frontend shows |
|---|---|---|---|
| Field errors | Input invalid | 400 ProblemDetails, `code: "validation_failed"`, `errors: { field: [messages] }` | Message under each field |
| Error alert | Request refused (not found, conflict, business rule, forbidden, rate limited) | ProblemDetails with `code`, `detail`, `correlationId` | Alert/toast with `detail`; reference id = `correlationId` |
| Inline messages | Success with information or a caveat | `messages: UserMessage[]` in the response contract | Toast per message by `severity` |
| Notifications | Something the user should know later | Stored row in `notify.Notification` | Bell icon with unread count, notification list |

- Every error response has a stable machine `code` (`not_found`, `conflict`, `duplicate`, `business_rule`,
  `rate_limited`, `internal_error`, or a specific one like `sales.discount.limit_exceeded`) and a `detail`
  that is safe to show. The frontend translates by `code` and falls back to `detail`.
- `MessageSeverity`: `Info`, `Success`, `Warning`, `Error` (stored as TINYINT 0 to 3; values never change).
- 500 responses never include exception text; support finds the log entry by `correlationId`.
- Notifications: any module calls `INotificationPublisher.PublishAsync(NotificationDraft)`. The draft is
  validated, then stored through `notify.usp_Notification_Create`. Users read and mark their own notifications
  only (`usp_Notification_List`, `usp_Notification_CountUnread`, `usp_Notification_MarkRead`), protected by RLS.

## Later (not built yet)

- HTTP endpoints for notifications: with authentication (ADR 0014).
- Real-time push to open browsers (SignalR) and e-mail/SMS/WhatsApp delivery through an outbox table
  processed by a background job, with retries and delivery status.
- Per-user notification preferences.

## Consequences

- The frontend needs one error handler and one notification component for the whole product.
- Message texts are English defaults; translation is keyed by `code`.
