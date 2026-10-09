using System.Net;
using System.Text.Json;
using FastXGen.Erp.IntegrationTests.Api.Probe;

namespace FastXGen.Erp.IntegrationTests.Api;

public sealed class RateLimitingTests(RateLimitedApiFactory factory) : IClassFixture<RateLimitedApiFactory>
{
    [Fact]
    public async Task Requests_over_the_limit_get_429_problem_details()
    {
        var client = factory.CreateClient();
        HttpResponseMessage? last = null;

        for (var i = 0; i <= RateLimitedApiFactory.Limit; i++)
        {
            last = await client.GetAsync("/api/v1/system/ping", TestContext.Current.CancellationToken);
        }

        Assert.Equal(HttpStatusCode.TooManyRequests, last!.StatusCode);
        using var body = JsonDocument.Parse(await last.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Equal("rate_limited", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Health_checks_are_never_rate_limited()
    {
        var client = factory.CreateClient();

        for (var i = 0; i <= RateLimitedApiFactory.Limit * 3; i++)
        {
            var response = await client.GetAsync("/health/live", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
