using System.Reflection;
using NetArchTest.Rules;
using ArchTestResult = NetArchTest.Rules.TestResult;

namespace FastXGen.Erp.ArchitectureTests;

/// <summary>
/// Enforces the dependency rule in docs/adr/0001. If one of these fails, fix the code,
/// not the test; changing a rule needs a new ADR.
/// </summary>
public sealed class LayerDependencyTests
{
    private const string Domain = "FastXGen.Erp.Domain";
    private const string Contracts = "FastXGen.Erp.Contracts";
    private const string Application = "FastXGen.Erp.Application";
    private const string Infrastructure = "FastXGen.Erp.Infrastructure";
    private const string Persistence = "FastXGen.Erp.Persistence";
    private const string Api = "FastXGen.Erp.Api";

    private const string Dapper = "Dapper";
    private const string SqlClient = "Microsoft.Data.SqlClient";
    private const string AspNetCore = "Microsoft.AspNetCore";

    [Fact]
    public void Domain_depends_on_nothing()
    {
        AssertNoDependency(Domain, Application, Contracts, Infrastructure, Persistence, Api, Dapper, SqlClient, AspNetCore);
    }

    [Fact]
    public void Contracts_depend_on_nothing()
    {
        AssertNoDependency(Contracts, Domain, Application, Infrastructure, Persistence, Api, Dapper, SqlClient, AspNetCore);
    }

    [Fact]
    public void Application_does_not_depend_on_outer_layers_or_data_access()
    {
        AssertNoDependency(Application, Infrastructure, Persistence, Api, Dapper, SqlClient, AspNetCore);
    }

    [Fact]
    public void Infrastructure_does_not_touch_the_database_or_http()
    {
        AssertNoDependency(Infrastructure, Persistence, Api, Dapper, SqlClient, AspNetCore);
    }

    [Fact]
    public void Persistence_does_not_depend_on_infrastructure_or_http()
    {
        AssertNoDependency(Persistence, Infrastructure, Api, AspNetCore);
    }

    [Fact]
    public void Only_the_sql_executor_namespace_uses_dapper_or_sqlclient()
    {
        var result = Types.InAssemblies([Load(Persistence), Load(Infrastructure), Load(Application), Load(Api)])
            .That().DoNotResideInNamespace(Persistence + ".Sql")
            .ShouldNot().HaveDependencyOnAny(Dapper, SqlClient)
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Controllers_call_application_only()
    {
        var result = Types.InAssembly(Load(Api))
            .That().ResideInNamespace(Api + ".Controllers")
            .ShouldNot().HaveDependencyOnAny(Persistence, Infrastructure, Dapper, SqlClient)
            .GetResult();

        AssertSuccess(result);
    }

    private static void AssertNoDependency(string layer, params string[] forbidden)
    {
        var result = Types.InAssembly(Load(layer))
            .ShouldNot().HaveDependencyOnAny(forbidden)
            .GetResult();

        AssertSuccess(result);
    }

    private static Assembly Load(string name) => Assembly.Load(name);

    private static void AssertSuccess(ArchTestResult result) =>
        Assert.True(
            result.IsSuccessful,
            "Architecture rule broken by: " + string.Join(", ", result.FailingTypeNames ?? []));
}
