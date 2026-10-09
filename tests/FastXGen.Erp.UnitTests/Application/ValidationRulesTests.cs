using FastXGen.Erp.Application.Common.Validation;
using FluentValidation;

namespace FastXGen.Erp.UnitTests.Application;

public sealed class ValidationRulesTests
{
    [Theory]
    [InlineData("Coca-Cola 500ml")]
    [InlineData("Ünïcödé café, 50% off & more")]
    [InlineData("Line one\r\nLine two\tTabbed")]
    public void PlainText_accepts_normal_text(string value)
    {
        Assert.True(Validate(new Sample { Text = value }).IsValid);
    }

    [Theory]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("name\u0000with-null")]
    [InlineData("bell\u0007")]
    [InlineData("invoice‮txt.exe")]
    public void PlainText_rejects_markup_control_and_spoofing_characters(string value)
    {
        Assert.False(Validate(new Sample { Text = value }).IsValid);
    }

    [Fact]
    public void PlainText_enforces_maximum_length()
    {
        Assert.False(Validate(new Sample { Text = new string('a', 51) }).IsValid);
    }

    [Theory]
    [InlineData("SKU-1001", true)]
    [InlineData("branch_03.main", true)]
    [InlineData("-starts-with-dash", false)]
    [InlineData("has space", false)]
    [InlineData("quote'", false)]
    public void Code_allows_only_safe_characters(string value, bool expected)
    {
        Assert.Equal(expected, Validate(new Sample { Code = value }).IsValid);
    }

    [Theory]
    [InlineData("0", true)]
    [InlineData("1234.5678", true)]
    [InlineData("99999999999999.9999", true)]
    [InlineData("-0.01", false)]
    [InlineData("1.23456", false)]
    [InlineData("100000000000000", false)]
    public void Money_fits_decimal_18_4_and_is_not_negative(string value, bool expected)
    {
        Assert.Equal(expected, Validate(new Sample { Amount = decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture) }).IsValid);
    }

    [Theory]
    [InlineData("0", false)]
    [InlineData("0.0001", true)]
    [InlineData("-1", false)]
    public void Quantity_must_be_positive(string value, bool expected)
    {
        Assert.Equal(expected, Validate(new Sample { Quantity = decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture) }).IsValid);
    }

    [Theory]
    [InlineData(0L, false)]
    [InlineData(-5L, false)]
    [InlineData(1L, true)]
    public void EntityId_must_be_positive(long value, bool expected)
    {
        Assert.Equal(expected, Validate(new Sample { Id = value }).IsValid);
    }

    [Fact]
    public void EntityKey_rejects_the_empty_guid()
    {
        Assert.False(Validate(new Sample { Key = Guid.Empty }).IsValid);
        Assert.True(Validate(new Sample { Key = Guid.NewGuid() }).IsValid);
    }

    [Theory]
    [InlineData("/inventory/products/42", true)]
    [InlineData("/sales/invoices?status=open", true)]
    [InlineData("https://evil.example/phish", false)]
    [InlineData("//evil.example", false)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("/path/<script>", false)]
    public void AppLink_allows_only_links_inside_the_application(string value, bool expected)
    {
        Assert.Equal(expected, Validate(new Sample { Link = value }).IsValid);
    }

    private static FluentValidation.Results.ValidationResult Validate(Sample sample) => new SampleValidator().Validate(sample);

    private sealed class Sample
    {
        public string? Text { get; init; }

        public string? Code { get; init; }

        public decimal Amount { get; init; }

        public decimal Quantity { get; init; } = 1m;

        public long Id { get; init; } = 1;

        public Guid Key { get; init; } = Guid.NewGuid();

        public string? Link { get; init; }
    }

    private sealed class SampleValidator : AbstractValidator<Sample>
    {
        public SampleValidator()
        {
            RuleFor(sample => sample.Text).PlainText(50);
            RuleFor(sample => sample.Code).Code(30);
            RuleFor(sample => sample.Amount).Money();
            RuleFor(sample => sample.Quantity).Quantity();
            RuleFor(sample => sample.Id).EntityId();
            RuleFor(sample => sample.Key).EntityKey();
            RuleFor(sample => sample.Link).AppLink();
        }
    }
}
