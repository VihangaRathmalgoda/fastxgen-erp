using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FastXGen.Erp.Api.Middleware;
using FastXGen.Erp.Contracts.System;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FastXGen.Erp.IntegrationTests.Api;

/// <summary>
/// Runs the real HTTP pipeline in memory. These tests need no database.
/// </summary>
public sealed class ApiSmokeTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Ping_returns_ok()
    {
        var response = await _client.GetFromJsonAsync<PingResponse>("/api/v1/system/ping", TestContext.Current.CancellationToken);

        Assert.Equal("ok", response!.Status);
    }

    [Fact]
    public async Task Liveness_check_does_not_need_the_database()
    {
        var response = await _client.GetAsync("/health/live", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Every_response_carries_a_correlation_id()
    {
        var response = await _client.GetAsync("/api/v1/system/ping", TestContext.Current.CancellationToken);

        Assert.True(response.Headers.TryGetValues(CorrelationIdMiddleware.HeaderName, out var values));
        Assert.False(string.IsNullOrWhiteSpace(values.Single()));
    }

    [Fact]
    public async Task Safe_incoming_correlation_id_is_kept()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/system/ping");
        request.Headers.Add(CorrelationIdMiddleware.HeaderName, "pos-terminal-0042");

        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal("pos-terminal-0042", response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single());
    }

    [Fact]
    public async Task Unsafe_incoming_correlation_id_is_replaced()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/system/ping");
        request.Headers.Add(CorrelationIdMiddleware.HeaderName, "<script>");

        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.NotEqual("<script>", response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single());
    }

    [Fact]
    public async Task Errors_are_returned_as_problem_details_with_correlation_id()
    {
        var response = await _client.GetAsync("/api/v1/does-not-exist", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.True(body.RootElement.TryGetProperty(CorrelationIdMiddleware.ItemKey, out _));
    }
}
