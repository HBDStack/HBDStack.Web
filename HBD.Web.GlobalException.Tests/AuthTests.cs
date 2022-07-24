using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using Web.Tests;

namespace HBD.Web.GlobalException.Tests;

public class AuthTests
{
    private HttpClient? _client;

    [OneTimeSetUp]
    public void Setup()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT","Development");

        var application = new WebApplicationFactory<Program>();

        _client = application.CreateClient();
        _client.DefaultRequestHeaders.Add("X-ARR-ClientCert", "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");
    }

    [OneTimeTearDown]
    public void Cleanup() => _client?.Dispose();
    
    [Test]
    public async Task CertAuthTest()
    {
        var rs = await _client!.GetAsync("/Roles");
        var content = await rs.Content.ReadAsStringAsync();
        
        Console.WriteLine(content);
        rs.StatusCode.Should().Be(HttpStatusCode.OK);
       
    }

}