using FastXGen.Erp.Application.Common.Validation;
using FastXGen.Erp.Contracts.Common;

namespace FastXGen.Erp.UnitTests.Application;

public sealed class PageRequestValidatorTests
{
    private readonly PageRequestValidator _validator = new();

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 200)]
    [InlineData(500, 50)]
    public void Accepts_valid_pages(int page, int pageSize)
    {
        Assert.True(_validator.Validate(new PageRequest(page, pageSize)).IsValid);
    }

    [Theory]
    [InlineData(0, 50)]
    [InlineData(-1, 50)]
    [InlineData(1, 0)]
    [InlineData(1, 201)]
    [InlineData(1, int.MaxValue)]
    public void Rejects_out_of_range_pages(int page, int pageSize)
    {
        Assert.False(_validator.Validate(new PageRequest(page, pageSize)).IsValid);
    }
}
