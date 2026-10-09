using FastXGen.Erp.Persistence.Sql;

namespace FastXGen.Erp.UnitTests.Persistence;

public sealed class StoredProcedureNameTests
{
    [Theory]
    [InlineData("sales.usp_Invoice_Create")]
    [InlineData("inventory.usp_StockMovement_Post")]
    [InlineData("core.usp_Health_Get")]
    public void Accepts_schema_usp_entity_action(string name)
    {
        Assert.Equal(name, StoredProcedureName.From(name).Value);
    }

    [Theory]
    [InlineData("usp_Invoice_Create")]
    [InlineData("dbo.Invoice_Create")]
    [InlineData("Sales.usp_Invoice_Create")]
    [InlineData("sales.usp_invoice_create")]
    [InlineData("sales.usp_Invoice")]
    [InlineData("SELECT * FROM sales.Invoice")]
    [InlineData("sales.usp_Invoice_Create; DROP TABLE x")]
    [InlineData("")]
    public void Rejects_anything_else(string name)
    {
        Assert.Throws<ArgumentException>(() => StoredProcedureName.From(name));
    }
}
