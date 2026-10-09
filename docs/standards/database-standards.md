# Database standards

Applies to everything under `database/FastXGen.Erp.Database`. Read with ADR 0002 to 0008.

## 1. Layout

One object per file. The folder is the schema; the file name is the object name without schema.

```
database/FastXGen.Erp.Database/
  <schema>/<schema>.sql                 CREATE SCHEMA
  <schema>/Tables/<Table>.sql
  <schema>/Types/<Entity>LineType.sql   table-valued parameter types
  <schema>/Functions/fn_<Name>.sql
  <schema>/Views/vw_<Name>.sql
  <schema>/StoredProcedures/usp_<Entity>_<Action>.sql
  Scripts/Script.PostDeployment.sql     reference data only (idempotent MERGE)
```

Schemas (create each when its module starts):

| Schema | Module |
|---|---|
| `core` | Tenants, companies, business units, branches, warehouses, terminals, settings |
| `identity` | Users, roles, permissions, scopes |
| `catalog` | Products, variants, units, barcodes, price lists |
| `inventory` | Stock movements, balances, transfers, counts |
| `sales` | Sales, lines, payments, returns, shifts |
| `purchasing` | Suppliers, purchase orders, GRNs, supplier invoices |
| `finance` | Accounts, journals, receivables, payables |
| `tax` | Tax profiles, rates, periods |
| `rules` | Business rules, versions, execution log |
| `report` | Report procedures and read models |
| `workflow` | Approval definitions and requests |
| `document` | Templates, numbering sequences |
| `audit` | Audit and security events |
| `notify` | In-app notifications (and later the delivery outbox) |
| `security` | RLS predicates and policy |
| `platform` | Super Admin: plans, features, tenant administration |

## 2. Naming

| Object | Pattern | Example |
|---|---|---|
| Table | singular PascalCase | `sales.Sale`, `sales.SaleLine` |
| Primary key column | `<Table>Id` (type per section 3) | `SaleId`, `UserId` |
| Display number | `DocumentNumber` | `INV-BR03-2026-000123` |
| Procedure | `usp_<Entity>_<Action>` | `sales.usp_Sale_Create`, `catalog.usp_Product_List` |
| Type (TVP) | `<Entity>Type` | `sales.SaleLineType` |
| Function | `fn_<Name>` | `security.fn_TenantAccessPredicate` |
| View | `vw_<Name>` | `report.vw_DailySales` |
| Primary key | `PK_<Table>` | `PK_Sale` |
| Foreign key | `FK_<Table>_<ReferencedTable>` | `FK_SaleLine_Sale` |
| Unique | `UQ_<Table>_<Columns>` | `UQ_Product_Sku` |
| Index | `IX_<Table>_<Columns>` | `IX_Sale_BranchId_SaleDate` |
| Default | `DF_<Table>_<Column>` | `DF_Product_IsActive` |
| Check | `CK_<Table>_<Rule>` | `CK_SaleLine_QuantityPositive` |

Standard actions: `Create`, `Update`, `Get` (one row), `List` (paged), `Search`, `Post` (ledger), `Void`,
`Approve`, `Deactivate`. Pick from this list before inventing a new verb.

## 3. Table templates

Which key a table gets (ADR 0006):

| Table holds | Key column | Type |
|---|---|---|
| Identity records: users, roles, sessions, terminals, API keys | `<Entity>Id` | `UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID()` |
| Business records: products, sales, lines, movements, payments | `<Entity>Id` | `BIGINT IDENTITY (1, 1)` |
| Documents people see (invoice, receipt, PO, GRN) | `<Entity>Id` + `DocumentNumber` | `BIGINT` + `NVARCHAR (30)` from `document.usp_DocumentNumber_Next` |
| Who did it (`CreatedByUserId`, `ApprovedByUserId`) | | `UNIQUEIDENTIFIER` |

### Business record

```sql
CREATE TABLE [catalog].[Product]
(
    [TenantId]        INT              NOT NULL,
    [ProductId]       BIGINT           IDENTITY (1, 1) NOT NULL,
    [Sku]             NVARCHAR (50)    NOT NULL,
    [Name]            NVARCHAR (200)   NOT NULL,
    [IsActive]        BIT              NOT NULL CONSTRAINT [DF_Product_IsActive] DEFAULT (1),
    [CreatedAtUtc]    DATETIME2 (3)    NOT NULL CONSTRAINT [DF_Product_CreatedAtUtc] DEFAULT (SYSUTCDATETIME()),
    [CreatedByUserId] UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAtUtc]    DATETIME2 (3)    NULL,
    [UpdatedByUserId] UNIQUEIDENTIFIER NULL,
    [RowVersion]      ROWVERSION       NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([TenantId], [ProductId]),
    CONSTRAINT [UQ_Product_Sku] UNIQUE ([TenantId], [Sku])
);
```

### Identity record

```sql
CREATE TABLE [identity].[User]
(
    [TenantId]           INT              NOT NULL,
    [UserId]             UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_User_UserId] DEFAULT (NEWSEQUENTIALID()),
    [UserName]           NVARCHAR (100)   NOT NULL,
    [NormalizedUserName] NVARCHAR (100)   NOT NULL,
    -- ...
    [RowVersion]         ROWVERSION       NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([TenantId], [UserId]),
    CONSTRAINT [UQ_User_NormalizedUserName] UNIQUE ([TenantId], [NormalizedUserName])
);
```

The insert procedure returns the generated key with `OUTPUT`, because `SCOPE_IDENTITY()` only works for
`IDENTITY` columns:

```sql
    INSERT INTO [identity].[User] ([TenantId], [UserName], [NormalizedUserName], ...)
    OUTPUT INSERTED.[UserId]
    VALUES (@TenantId, @UserName, @NormalizedUserName, ...);
```

Then add the table to `security/SecurityPolicies/TenantIsolationPolicy.sql`:

```sql
ADD FILTER PREDICATE [security].[fn_TenantAccessPredicate]([TenantId]) ON [catalog].[Product],
ADD BLOCK  PREDICATE [security].[fn_TenantAccessPredicate]([TenantId]) ON [catalog].[Product],
```

Rules: `TenantId` first in every key and index; explicit lengths; `DECIMAL(18,4)` for money and quantity;
no `float`, `real`, `money`, `text`, `ntext`, `datetime`; foreign keys always declared;
transaction tables (sales, movements) have no `UpdatedAtUtc` because they are never updated (ADR 0008).

## 4. Procedure templates

Every procedure starts with the same header. The tenant comes from `SESSION_CONTEXT`, never a parameter.

### Write (create) with idempotency, lines and audit

```sql
CREATE PROCEDURE [sales].[usp_Sale_Create]
    @BranchId        BIGINT,
    @TerminalId      UNIQUEIDENTIFIER,
    @CustomerId      BIGINT = NULL,
    @SaleDate        DATE,
    @TotalAmount     DECIMAL (18, 4),
    @ClientRequestId UNIQUEIDENTIFIER,
    @Lines           [sales].[SaleLineType] READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @TenantId INT    = CAST(SESSION_CONTEXT(N'TenantId') AS INT);
    DECLARE @UserId   UNIQUEIDENTIFIER = CAST(SESSION_CONTEXT(N'UserId') AS UNIQUEIDENTIFIER);

    IF @TenantId IS NULL OR @UserId IS NULL
        THROW 50403, N'No tenant or user context.', 1;

    -- Idempotency: a retried request returns the original result instead of posting twice.
    DECLARE @SaleId BIGINT =
    (
        SELECT [SaleId] FROM [sales].[Sale]
        WHERE [TenantId] = @TenantId AND [ClientRequestId] = @ClientRequestId
    );

    IF @SaleId IS NOT NULL
    BEGIN
        SELECT @SaleId AS [SaleId];
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM @Lines)
        THROW 50422, N'A sale needs at least one line.', 1;

    BEGIN TRANSACTION;

    INSERT INTO [sales].[Sale] ([TenantId], [BranchId], [TerminalId], [CustomerId], [SaleDate],
                                [TotalAmount], [ClientRequestId], [CreatedByUserId])
    VALUES (@TenantId, @BranchId, @TerminalId, @CustomerId, @SaleDate,
            @TotalAmount, @ClientRequestId, @UserId);

    SET @SaleId = CAST(SCOPE_IDENTITY() AS BIGINT);

    INSERT INTO [sales].[SaleLine] ([TenantId], [SaleId], [LineNumber], [ProductId], [Quantity], [UnitPrice], [LineTotal])
    SELECT @TenantId, @SaleId, [LineNumber], [ProductId], [Quantity], [UnitPrice], [LineTotal]
    FROM @Lines;

    EXEC [audit].[usp_AuditEvent_Write]
        @Action     = N'Sales.Sale.Create',
        @EntityType = N'Sale',
        @EntityId   = @SaleId;

    COMMIT TRANSACTION;

    SELECT @SaleId AS [SaleId];
END;
```

### Update with optimistic concurrency

```sql
    BEGIN TRANSACTION;

    UPDATE [catalog].[Product]
    SET    [Name] = @Name, [UpdatedAtUtc] = SYSUTCDATETIME(), [UpdatedByUserId] = @UserId
    WHERE  [TenantId] = @TenantId AND [ProductId] = @ProductId AND [RowVersion] = @RowVersion;

    IF @@ROWCOUNT = 0
    BEGIN
        IF EXISTS (SELECT 1 FROM [catalog].[Product] WHERE [TenantId] = @TenantId AND [ProductId] = @ProductId)
            THROW 50409, N'This product was changed by someone else. Reload and try again.', 1;

        THROW 50404, N'Product not found.', 1;
    END;

    -- audit, then COMMIT
```

### Paged list

```sql
CREATE PROCEDURE [catalog].[usp_Product_List]
    @Search   NVARCHAR (100) = NULL,
    @Page     INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TenantId INT = CAST(SESSION_CONTEXT(N'TenantId') AS INT);
    IF @TenantId IS NULL
        THROW 50403, N'No tenant context.', 1;

    IF @Page < 1 OR @PageSize NOT BETWEEN 1 AND 200
        THROW 50422, N'Page must be 1 or more and page size between 1 and 200.', 1;

    SELECT [ProductId], [Sku], [Name], [IsActive], [RowVersion],
           COUNT(*) OVER () AS [TotalCount]
    FROM   [catalog].[Product]
    WHERE  [TenantId] = @TenantId
      AND  (@Search IS NULL OR [Sku] LIKE @Search + N'%' OR [Name] LIKE N'%' + @Search + N'%')
    ORDER BY [Name], [ProductId]
    OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
```

## 5. Procedure rules

- `SET NOCOUNT ON` always; `SET XACT_ABORT ON` in every procedure that writes.
- Read tenant and user from `SESSION_CONTEXT`; throw 50403 if missing.
- Filter every query by `TenantId` even though RLS also does: RLS is the safety net, not the plan.
- Enforce branch / own-records scope in the `WHERE` clause when the caller's scope is limited.
- Error numbers: 50403 forbidden, 50404 not found, 50409 conflict, 50422 business rule (ADR 0007).
  Messages are user-facing; no table or constraint names.
- No dynamic SQL (`sp_executesql`, `EXEC(@sql)`), no cursors, no `SELECT *`, no `NOLOCK` hints
  (the database uses read-committed snapshot).
- Writes return the new id or the new `RowVersion` as a single-row result set.
- Lists are always paged; result columns use the same names as the C# record properties.
- A procedure does one thing. Shared steps go in another procedure (`audit.usp_AuditEvent_Write`), not copied.
- Business decisions stay in C# (ADR 0005).

## 6. Changing the database

1. Edit or add the `.sql` file(s). Build: `dotnet build database/FastXGen.Erp.Database`. It must pass with no warnings.
2. Never rename or drop a column in one step: add the new column, migrate data with a pre-deployment script,
   switch code, remove the old column in a later release.
3. On a personal machine, apply it to your own local database: `./scripts/Publish-LocalDatabase.ps1`
   (use `-ScriptOnly` to read the generated SQL first). Never on an office laptop.
4. The PR shows the `.sql` diff; a second person reviews locking and data impact on large tables.
5. Deployment scripts for shared environments are generated from the build (ADR 0003) and run by the person
   authorised for that environment. Developers never run them against shared servers.
