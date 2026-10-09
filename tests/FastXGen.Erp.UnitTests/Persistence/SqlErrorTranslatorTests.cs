using FastXGen.Erp.Application.Common.Exceptions;
using FastXGen.Erp.Persistence.Sql;

namespace FastXGen.Erp.UnitTests.Persistence;

public sealed class SqlErrorTranslatorTests
{
    [Theory]
    [InlineData(50403, typeof(ForbiddenException))]
    [InlineData(50404, typeof(NotFoundException))]
    [InlineData(50409, typeof(ConflictException))]
    [InlineData(50422, typeof(BusinessRuleException))]
    [InlineData(2601, typeof(ConflictException))]
    [InlineData(2627, typeof(ConflictException))]
    public void Known_error_numbers_map_to_application_exceptions(int errorNumber, Type expected)
    {
        var exception = SqlErrorTranslator.Translate(errorNumber, "message");

        Assert.IsType(expected, exception);
    }

    [Fact]
    public void Procedure_message_is_kept_for_thrown_business_errors()
    {
        var exception = SqlErrorTranslator.Translate(SqlErrorTranslator.BusinessRule, "Discount exceeds limit.");

        Assert.Equal("Discount exceeds limit.", exception!.Message);
    }

    [Fact]
    public void Duplicate_key_message_does_not_leak_sql_details()
    {
        var exception = SqlErrorTranslator.Translate(2627, "Violation of UNIQUE KEY constraint 'UQ_Product_Sku'.");

        Assert.DoesNotContain("UQ_Product_Sku", exception!.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(1205)]
    [InlineData(547)]
    [InlineData(50000)]
    public void Unexpected_errors_are_not_translated(int errorNumber)
    {
        Assert.Null(SqlErrorTranslator.Translate(errorNumber, "message"));
    }
}
