using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HBD.AspNetCore.Tests;

[TestClass]
public class RateLimitTests
{
    private static HttpClient _client = default!;
    

    [ClassInitialize]
    public static void Setup(TestContext _)
    {
        var builder = new WebHostBuilder()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseStartup<Startup>()
            .UseEnvironment("Testing");

        var server = new TestServer(builder)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };
        _client = server.CreateClient();

        // client always expects json results
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    [TestMethod]
    public async Task AlwaysAbleToRetryTheFailedRequestAsync()
    {
        for (var i = 0; i < 5; i++)
        {
            var response = await _client.GetAsync("/RateLimits/Id");
            response.StatusCode.Should().NotBe(HttpStatusCode.TooManyRequests);
        }
    }

    [TestMethod]
    public async Task TestRateLimitWithManyCallsAsync()
    {
        var response = await _client.GetAsync("/RateLimits");
        response.StatusCode.Should().NotBe(HttpStatusCode.TooManyRequests);

        response = await _client.GetAsync("/RateLimits");
        response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }

    [TestMethod]
    public async Task TestRateLimitWithManyEvery3sAsync()
    {
        for (var i = 0; i < 5; i++)
        {
            await Task.Delay((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
            var response = await _client.GetAsync("/RateLimits");
            response.StatusCode.Should().NotBe(HttpStatusCode.TooManyRequests);
        }
    }
}