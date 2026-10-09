using FastXGen.Erp.Application;
using FastXGen.Erp.Contracts.Common;
using FluentValidation;

namespace FastXGen.Erp.ArchitectureTests;

/// <summary>
/// Every request type a client can send must have a validator (ADR 0012). The API also fails closed at
/// runtime, but this catches a missing validator at build time.
/// </summary>
public sealed class ValidationCoverageTests
{
    [Fact]
    public void Every_request_contract_has_a_validator()
    {
        var validated = typeof(ApplicationRegistration).Assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsClass: true })
            .SelectMany(type => type.GetInterfaces())
            .Where(contract => contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IValidator<>))
            .Select(contract => contract.GetGenericArguments()[0])
            .ToHashSet();

        var missing = typeof(PageRequest).Assembly.GetTypes()
            .Where(type => type.IsClass && type.Name.EndsWith("Request", StringComparison.Ordinal))
            .Where(type => !validated.Contains(type))
            .Select(type => type.FullName)
            .ToList();

        Assert.True(missing.Count == 0, "Request contracts without a validator: " + string.Join(", ", missing));
    }
}
