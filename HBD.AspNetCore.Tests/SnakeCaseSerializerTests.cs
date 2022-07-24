using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBD.AspNetCore.Tests;

[TestClass]
public class SnakeCaseSerializerTests
{
    private static HttpClient _client = default!;

    [ClassInitialize]
    public static void Setup(TestContext _)
    {
        var builder = new WebHostBuilder()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseStartup<Startup>()
            .UseEnvironment("SnakeCaseTesting");

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
    public async Task GetById_ThrowBadRequestError()
    {
        var response = await _client.GetAsync($"/problems/{Guid.Empty}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        content.Should().NotBeNullOrEmpty();
        content.Should().ContainAll("\"status\":400", "trace_id", "error_code", "error_message", "error_details",
            "error_code", "references", "error_message", "transactionId");
    }

    [TestMethod]
    public async Task Post_ThrowArgumentNullExceptionError()
    {
        var response = await _client.PostAsJsonAsync("/problems", "argNullError");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        content.Should().ContainAll("status", "trace_id", "\"error_code\":\"BadRequest\"",
            "\"error_message\":\"One or more validation errors occurred.\"",
            "\"error_details\":{\"errorName\":[{\"error_code\":\"invalid_argument\",\"references\":null,\"error_message\":\"Field is required (Parameter 'errorName')\"}]}");
    }

    [TestMethod]
    public async Task Post_ThrowArgumentExceptionError()
    {
        var response = await _client.PostAsJsonAsync("/problems", "argError");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        content.Should().ContainAll("status", "trace_id",
            "\"error_code\":\"BadRequest\"",
            "\"error_message\":\"Field is required\"",
            "\"error_details\":null");
    }

    [TestMethod]
    public async Task Post_ThrowValidationExceptionError()
    {
        var response = await _client.PostAsJsonAsync("/problems", "validationError");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        content.Should().ContainAll("status", "trace_id",
            "\"error_code\":\"BadRequest\"",
            "\"error_message\":\"One or more validation errors occurred.\"",
            "\"error_details\":{\"FirstName\":[{\"error_code\":\"invalid_argument\",\"references\":null,\"error_message\":\"Field is required\"}],\"LastName\":[{\"error_code\":\"invalid_argument\",\"references\":null,\"error_message\":\"Field is required\"}]}");
    }

    [TestMethod]
    public async Task Post_ThrowAggregateExceptionError()
    {
        var response = await _client.PostAsJsonAsync("/problems", "aggregateError");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        content.Should().ContainAll("status", "trace_id",
            "\"error_code\":\"BadRequest\"",
            "\"error_message\":\"System cannot process this request (Inner error) (Field is required (Parameter 'errorName'))\"",
            "\"error_details\":null");
    }

    [TestMethod]
    public async Task Post_ThrowApiExceptionError()
    {
        var response = await _client.PostAsJsonAsync("/problems", "refitError1");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        content.Should().ContainAll("status", "trace_id",
            "\"error_code\":\"BadRequest\"",
            "\"error_message\":\"System cannot process this request\"",
            "\"error_details\":null");
    }
}