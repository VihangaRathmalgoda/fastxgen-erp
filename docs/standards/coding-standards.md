# C# coding standards

The build enforces most of this (`.editorconfig`, analyzers, warnings as errors, architecture tests).
This page covers what tools cannot check.

## Structure

- Feature folders, not technical folders: `Application/Features/Sales/CreateSale/` holds the command,
  its handler, its validator and nothing else.
- One public type per file; the file name matches the type.
- A use case is one class with one public `HandleAsync` method. No "service" classes with 20 methods.
- Interfaces (ports) that Application needs live in `Application/Abstractions/<Area>`.
- Repositories live in `Persistence/Repositories/<Module>`, are `internal sealed`, and call `ISqlExecutor` only.
- Keep types `internal` unless another project needs them. Prefer `sealed` classes and `record` for data.

## Naming

- Names describe the business: `PostStockMovement`, not `ProcessData`.
- Forbidden suffixes: `Helper`, `Manager`, `Utils`, `Util`, `Common`, `Misc`, `Base2`.
- Async methods end in `Async` and take a `CancellationToken` as the last parameter.
- Private fields `_camelCase`; constants and static readonly `PascalCase`.

## Code

- Methods short enough to read without scrolling; extract when a block needs a comment to explain *what* it does.
- Comments explain *why*, never *what*. No commented-out code. No `TODO` without an issue number.
- Guard clauses over nested `if`s. No `else` after `return`/`throw`.
- Money and quantity are `decimal`; rounding is always explicit (`Math.Round(value, 2, MidpointRounding.AwayFromZero)`).
- Time comes from `IClock`, never `DateTime.Now` / `DateTimeOffset.UtcNow` in business code.
- Never catch `Exception` to hide it. Expected failures throw `AppException` subtypes.
- No static mutable state. Everything through constructor injection (primary constructors are fine).
- No `async void`, no `.Result` or `.Wait()`.

## Validation (ADR 0012)

- Each request contract gets a validator next to its use case:
  `Application/Features/<Module>/<UseCase>/<Name>RequestValidator.cs`, `internal sealed`, using `ValidationRules`.

  ```csharp
  internal sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
  {
      public CreateProductRequestValidator()
      {
          RuleFor(request => request.Sku).NotEmpty().Code(50);
          RuleFor(request => request.Name).NotEmpty().PlainText(200);
          RuleFor(request => request.Price).Money();
          RuleFor(request => request.CategoryId).EntityId();
      }
  }
  ```
- Validators check shape and ranges only. Rules that need data ("does this SKU already exist?",
  "is this discount within the user's limit?") belong in the use case or the procedure, and fail with
  `ConflictException` / `BusinessRuleException`.
- Every string has a maximum length that matches its database column.

## Messages (ADR 0013)

- Throw `NotFoundException`, `ConflictException`, `ForbiddenException` or `BusinessRuleException` with a
  `module.reason` code and a sentence a shop owner understands: "Discount is above your limit."
- Never put ids of other tenants, SQL, stack traces or internal names in a message.

## Tests

- Test code follows the same style; small fixture types used by one test class may share its file.

## Dependencies

- Package versions only in `Directory.Packages.props`.
- Ask for approval before adding a package. State: what it solves, why the framework is not enough,
  licence, maintenance status.
- Not allowed without an ADR: EF Core, MediatR, AutoMapper, FluentAssertions (licence), any ORM.

## Logging

- Structured logging with message templates: `logger.LogInformation("Sale {SaleId} posted", saleId)`.
- Never log passwords, tokens, card data, connection strings or full personal data.
