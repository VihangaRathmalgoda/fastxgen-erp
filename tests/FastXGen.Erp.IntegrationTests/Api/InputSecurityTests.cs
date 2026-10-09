using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FastXGen.Erp.IntegrationTests.Api.Probe;

namespace FastXGen.Erp.IntegrationTests.Api;

/// <summary>
/// Proves the protections in ADR 0012 and the error contract in ADR 0013 through the real HTTP pipeline.
/// </summary>
public sealed class InputSecurityTests(ProbeApiFactory factory) : IClassFixture<ProbeApiFactory>
{
    private const string ProbeUrl = "/api/v1/test-probe";
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Valid_request_reaches_the_action()
    {
        var response = await _client.PostAsJsonAsync(ProbeUrl, new { name = "Main Street", code = "BR-01", amount = 10.5m }, Ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Invalid_fields_return_validation_problem_with_camel_case_field_names()
    {
        var response = await _client.PostAsJsonAsync(ProbeUrl, new { name = "<script>", code = "bad code", amount = -1m }, Ct);

        using var body = await ReadProblemAsync(response, HttpStatusCode.BadRequest, "validation_failed");
        var errors = body.RootElement.GetProperty("errors");
        Assert.True(errors.TryGetProperty("name", out _));
        Assert.True(errors.TryGetProperty("code", out _));
        Assert.True(errors.TryGetProperty("amount", out _));
    }

    [Fact]
    public async Task Unknown_json_properties_are_rejected()
    {
        var response = await _client.PostAsJsonAsync(ProbeUrl, new { name = "Shop", code = "BR-01", amount = 1m, isAdmin = true }, Ct);

        using var _ = await ReadProblemAsync(response, HttpStatusCode.BadRequest, "validation_failed");
    }

    [Fact]
    public async Task Malformed_json_returns_validation_problem_without_parser_details()
    {
        using var content = new StringContent("{ \"name\": \"Shop\", \"amount\": \"DROP TABLE\" ", Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(ProbeUrl, content, Ct);

        using var body = await ReadProblemAsync(response, HttpStatusCode.BadRequest, "validation_failed");
        Assert.DoesNotContain("System.Text.Json", body.RootElement.GetRawText(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Request_type_without_validator_is_refused_without_leaking_details()
    {
        var response = await _client.GetAsync(ProbeUrl + "/unvalidated?value=x", Ct);

        using var body = await ReadProblemAsync(response, HttpStatusCode.InternalServerError, "internal_error");
        Assert.DoesNotContain("validator", body.RootElement.GetRawText(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Business_rule_failure_returns_422_with_its_code_and_message()
    {
        var response = await _client.GetAsync(ProbeUrl + "/business-rule", Ct);

        using var body = await ReadProblemAsync(response, HttpStatusCode.UnprocessableEntity, "sales.discount.limit_exceeded");
        Assert.Equal("Discount is above your limit.", body.RootElement.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task Validation_failure_raised_inside_the_application_returns_400()
    {
        var response = await _client.GetAsync(ProbeUrl + "/internal-validation", Ct);

        using var body = await ReadProblemAsync(response, HttpStatusCode.BadRequest, "validation_failed");
        Assert.True(body.RootElement.GetProperty("errors").TryGetProperty("lines[0].unitPrice", out _));
    }

    [Fact]
    public async Task Responses_carry_security_headers_and_no_server_banner()
    {
        var response = await _client.GetAsync("/api/v1/system/ping", Ct);

        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
        Assert.Contains("default-src 'none'", response.Headers.GetValues("Content-Security-Policy").Single(), StringComparison.Ordinal);
        Assert.True(response.Headers.CacheControl?.NoStore);
        Assert.False(response.Headers.Contains("Server"));
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private static async Task<JsonDocument> ReadProblemAsync(HttpResponseMessage response, HttpStatusCode status, string code)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync(Ct));
        Assert.Equal(code, body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.TryGetProperty("correlationId", out _));
        return body;
    }
}
