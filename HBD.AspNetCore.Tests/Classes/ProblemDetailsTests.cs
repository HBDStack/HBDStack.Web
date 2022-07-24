using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using HBDStack.AspNetCore.ErrorHandlers;
using HBDStack.StatusGeneric;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBD.AspNetCore.Tests.Classes;

[TestClass]
public class ProblemDetailsTests
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
    public void Test_Serialization_Without_Errors()
    {
        var p = new ProblemDetails
        {
            Status = HttpStatusCode.BadRequest,
            ErrorMessage = "There are something wrong",
        };

        var rs = JsonSerializer.Serialize(p, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        rs.Should().NotContain(nameof(ProblemDetails.ErrorDetails));

        Console.WriteLine(rs);
    }

    [TestMethod]
    public void Test_Serialization_With_Errors()
    {
        var p = new ProblemDetails
        {
            Status = HttpStatusCode.BadRequest,
            ErrorMessage = "There are something wrong",
            ErrorDetails = new ProblemResultCollection
            {
                ["PhoneNumber"] = new List<ProblemResult>
                {
                    new("required", "Phone number is required."),
                    new("Phone number is invalid."),
                }
            }
        };

        var rs = JsonSerializer.Serialize(p, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        rs.Should().Contain(nameof(ProblemDetails.ErrorDetails));

        Console.WriteLine(rs);
    }


    [TestMethod]
    public void Test_Serialization_StatusGeneric_ProblemDetails()
    {
        var status = new StatusGenericHandler();
        status.AddError("There are somethings wrong.");
        status.AddValidationResult(new ValidationResult("The phone number is wrong", new[] { "PhoneNumber" }));
        status.AddValidationResult(new GenericValidationResult("The phone number is invalid",
            new[] { "PhoneNumber", "MobileNumber" }));

        var p = status.ToProblemDetails();
        //Check Problem Details
        p.ErrorDetails.Count.Should().Be(3);
        p.ErrorDetails["PhoneNumber"].Count.Should().Be(2);
        p.ErrorDetails["MobileNumber"].Count.Should().Be(1);
        p.ErrorDetails[string.Empty].Count.Should().Be(1);

        //Check Serialised String.
        var rs = JsonSerializer.Serialize(p, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        rs.Should().Contain(nameof(ProblemDetails.ErrorDetails));

        Console.WriteLine(rs);
    }

    [TestMethod]
    public async Task GetById_ThrowBadRequestError()
    {
        var response = await _client.GetAsync($"/problems/{Guid.Empty}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var problemDetails = JsonSerializerExtensions.Deserialize<ProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails?.ErrorCode.Should().Be("BadRequest");
        problemDetails?.ErrorDetails.Should().NotBeNull();
        problemDetails?.ErrorDetails.Should().BeEquivalentTo(new ProblemResultCollection
        {
            {
                "transactionId", new List<ProblemResult>
                {
                    new("invalid_argument", "Value cannot be null. (Parameter 'transactionId')")
                }
            }
        });
    }

    [TestMethod]
    public async Task Post_ThrowArgumentNullExceptionError()
    {
        var response = await _client.PostAsJsonAsync("/problems", "argNullError");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var problemDetails = JsonSerializerExtensions.Deserialize<ProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails?.ErrorCode.Should().Be("BadRequest");
        problemDetails?.ErrorMessage.Should().Be("One or more validation errors occurred.");
        problemDetails?.ErrorDetails.Should().NotBeNull();
        problemDetails?.ErrorDetails.Should().BeEquivalentTo(new ProblemResultCollection
        {
            {
                "errorName", new List<ProblemResult>
                {
                    new("invalid_argument", "Field is required (Parameter 'errorName')")
                }
            }
        });
    }

    [TestMethod]
    public async Task Post_ThrowArgumentExceptionError()
    {
        var response = await _client.PostAsJsonAsync("/problems", "argError");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var problemDetails = JsonSerializerExtensions.Deserialize<ProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails?.ErrorCode.Should().Be("BadRequest");
        problemDetails?.ErrorMessage.Should().Be("Field is required");
        problemDetails?.ErrorDetails.Should().BeNull();
    }

    [TestMethod]
    public async Task Post_ThrowInvalidArgumentExceptionError()
    {
        var response = await _client.PostAsJsonAsync("/problems", "argInvalidError");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var problemDetails = JsonSerializerExtensions.Deserialize<ProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails?.ErrorCode.Should().Be("BadRequest");
        problemDetails?.ErrorMessage.Should().Be("Invalid argument exception");
        problemDetails?.ErrorDetails.Should().BeNull();
    }

    [TestMethod]
    public async Task Post_ThrowValidationExceptionError()
    {
        var response = await _client.PostAsJsonAsync("/problems", "validationError");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        content.Should().ContainAll("\"status\":400", "\"errorCode\":\"BadRequest\"");

        var problemDetails = JsonSerializerExtensions.Deserialize<ProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails?.ErrorCode.Should().Be("BadRequest");
        problemDetails?.ErrorMessage.Should().Be("One or more validation errors occurred.");
        problemDetails?.ErrorDetails.Should().NotBeNull();
        problemDetails?.ErrorDetails.Should().BeEquivalentTo(new ProblemResultCollection
        {
            {
                "FirstName", new List<ProblemResult>
                {
                    new("invalid_argument", "Field is required")
                }
            },
            {
                "LastName", new List<ProblemResult>
                {
                    new("invalid_argument", "Field is required")
                }
            }
        });
    }

    [TestMethod]
    public async Task Post_ThrowAggregateExceptionError()
    {
        var response = await _client.PostAsJsonAsync("/problems", "aggregateError");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var problemDetails = JsonSerializerExtensions.Deserialize<ProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails?.ErrorCode.Should().Be("BadRequest");
        problemDetails?.ErrorMessage.Should()
            .Be("System cannot process this request (Inner error) (Field is required (Parameter 'errorName'))");
        problemDetails?.ErrorDetails.Should().BeNull();
    }

    [TestMethod]
    public async Task Post_ThrowApiExceptionError()
    {
        var response = await _client.PostAsJsonAsync("/problems", "refitError1");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var problemDetails = JsonSerializerExtensions.Deserialize<ProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails?.ErrorCode.Should().Be("BadRequest");
        problemDetails?.ErrorMessage.Should().Be("System cannot process this request");
        problemDetails?.ErrorDetails.Should().BeNull();
    }

    [TestMethod]
    [DataRow("refitError2", "One or more validation errors occurred.")]
    [DataRow("refitError3", "{\"error\":\"Unauthorized\",\"message\":\"One or more validation errors occurred.\"}")]
    public async Task Post_ThrowUnauthorizedApiExceptionError(string errorType, string expectedMessage)
    {
        var response = await _client.PostAsJsonAsync("/problems", errorType);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var problemDetails = JsonSerializerExtensions.Deserialize<ProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails?.ErrorCode.Should().Be("Unauthorized");
        problemDetails?.ErrorMessage.Should().Be(expectedMessage);
        problemDetails?.ErrorDetails.Should().BeNull();
    }

    [DataTestMethod]
    [DataRow("dbError", HttpStatusCode.InternalServerError)]
    [DataRow("appError", HttpStatusCode.BadRequest)]
    public async Task Post_ThrowInternalServerError(string error, HttpStatusCode expectedErrorCode)
    {
        var response = await _client.PostAsJsonAsync("/problems", error);
        response.StatusCode.Should().Be(expectedErrorCode);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var problemDetails = JsonSerializerExtensions.Deserialize<ProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails?.TraceId.Should().NotBeNull();
        problemDetails?.Status.Should().Be(expectedErrorCode);
        problemDetails?.ErrorCode.Should().Be(expectedErrorCode.ToString());
        problemDetails?.ErrorMessage.Should().NotBeNull();
    }

    [TestMethod]
    public async Task Post_ThrowBizError()
    {
        var response = await _client.PostAsJsonAsync("/problems/bizError", new { });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        content.Should().ContainAll("\"status\":400", "\"errorCode\":\"BadRequest\"");
        var problemDetails = JsonSerializerExtensions.Deserialize<ProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails?.TraceId.Should().NotBeNull();
        problemDetails?.Status.Should().Be(HttpStatusCode.BadRequest);
        problemDetails?.ErrorCode.Should().Be(HttpStatusCode.BadRequest.ToString());
        problemDetails?.ErrorMessage.Should().NotBeNull();
    }
}