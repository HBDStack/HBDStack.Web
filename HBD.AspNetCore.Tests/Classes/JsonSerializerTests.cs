using System;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using HBDStack.AspNetCore.ErrorHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBD.AspNetCore.Tests.Classes;

[TestClass]
public class JsonSerializerTests
{
    [TestMethod]
    public void Serialize_ThenDeserialize_Without_Errors()
    {
        var originJson = new JsonObject
        {
            Id = new Guid("94010593-c8bb-4f8e-a773-eaeb2426b830"),
            Name = "John Doe",
            Date = DateTimeOffset.Now
        };

        var jsonText = JsonSerializerExtensions.Serialize(originJson);
        jsonText.Should().NotBeNull();

        var jsonObject = JsonSerializerExtensions.Deserialize<JsonObject>(jsonText);
        jsonObject.Should().NotBeNull();
        jsonObject?.Id.Should().Be(originJson.Id);
        jsonObject?.Name.Should().Be(originJson.Name);
        jsonObject?.Date.Should().Be(originJson.Date);
    }

    [TestMethod]
    public void DeserializeProblemDetails_Success()
    {
        var problemString =
            "{\"status\":\"BadRequest\",\"traceId\":\"90f3eeb9ef828b418f023fdca845d6ba\",\"errorCode\":\"BadRequest\",\"errorMessage\":\"Failed with 1 error\",\"errorDetails\":{\"NOT_FOUND_TRANSFER_CAN_MATCH_WITH_BANKFEED\":[{\"errorMessage\":\"Not found transfer to reconcile with debit feed FeedNumber\"}]}}";

        var jsonObject = JsonSerializerExtensions.Deserialize<ProblemDetails>(problemString);
        jsonObject.Should().NotBeNull();
        jsonObject!.Status.Should().Be(HttpStatusCode.BadRequest);

        problemString =
            "{\"status\":\"403\",\"traceId\":\"90f3eeb9ef828b418f023fdca845d6ba\",\"errorCode\":\"BadRequest\",\"errorMessage\":\"Failed with 1 error\",\"errorDetails\":{\"NOT_FOUND_TRANSFER_CAN_MATCH_WITH_BANKFEED\":[{\"errorMessage\":\"Not found transfer to reconcile with debit feed FeedNumber\"}]}}";

        jsonObject = JsonSerializerExtensions.Deserialize<ProblemDetails>(problemString);
        jsonObject.Should().NotBeNull();
        jsonObject!.Status.Should().Be(HttpStatusCode.Forbidden);

        problemString =
            "{\"status\":401,\"traceId\":\"9c95f13ec08aea4ca4d3eeaf44108834\",\"errorCode\":\"BadRequest\",\"errorMessage\":\"Failed with 1 error\",\"errorDetails\":{\"DEPOSIT_STATUS_NOT_ALLOW_APPROVE\":[{\"errorMessage\":\"Deposit in status Expired cannot be approved\"}]}}";
        jsonObject = JsonSerializerExtensions.Deserialize<ProblemDetails>(problemString);
        jsonObject.Should().NotBeNull();
        jsonObject!.Status.Should().Be(HttpStatusCode.Unauthorized);
    }
}

internal class JsonObject
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTimeOffset Date { get; set; }
}