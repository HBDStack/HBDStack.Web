using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HBDStack.Web.RequestLogs.HttpClients;
using Microsoft.Extensions.DependencyInjection;

namespace HBD.Web.Logs.Storages.Sql.Tests;

public class HttpClientTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture _fixture;

    public HttpClientTests(ApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.CreateClient();
    }

    [Fact]
    public async Task CallLogsApi_Success()
    {
        var client = _fixture.CreateDefaultClient(_fixture.ScopeServices.ServiceProvider.GetRequiredService<RequestLogClientHandler>());

        var rs = await client.GetStringAsync("/Logs/Guids");
        rs.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CallLogsApi_Failed()
    {
        var client =  _fixture.CreateDefaultClient(_fixture.ScopeServices.ServiceProvider.GetRequiredService<RequestLogClientHandler>());

        var rs = await client.PostAsJsonAsync("/Logs/Guids", new { Name = "Hello" });
        rs.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }
}