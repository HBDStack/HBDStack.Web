using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using FluentAssertions;
using Web.Tests;

namespace HBD.Web.GlobalException.Tests;

public class GlobalExceptionTests
{
    private HttpClient? _client;

    [OneTimeSetUp]
    public void Setup()
    {
        var application = new WebApplicationFactory<Program>();

        _client = application.CreateClient();
    }

    [OneTimeTearDown]
    public void Cleanup() => _client?.Dispose();

    [Test]
    public async Task GetErrorStatus()
    {
        var rs = await _client!.GetAsync("/Exceptions/Status");

        rs.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await rs.Content.ReadAsStringAsync();

        body.Should().Contain("CustomGlobalExceptionHandler").And.Contain("BadRequest").And.Contain("Some error").And.Contain("Validation error");
    }
    
    [Test]
    public async Task GetExceptionStatus()
    {
        var rs = await _client!.GetAsync("/Exceptions/Exception");

        rs.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var body = await rs.Content.ReadAsStringAsync();

        body.Should().Contain("CustomGlobalExceptionHandler").And.Contain("InternalServerError").And.Contain("Invalid operation");
    }
}