using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HBD.AspNetCore.Tests.Exceptions;
using HBDStack.StatusGeneric;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Refit;

namespace HBD.AspNetCore.Tests.Controllers;

[Produces("application/json")]
[Route("[controller]")]
public class ProblemsController : ControllerBase
{
    [HttpGet("{transactionId:guid}")]
    public ActionResult GetById(Guid transactionId) => throw new ArgumentNullException(nameof(transactionId));

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] string errorName) => errorName switch
    {
        "argNullError" => throw new ArgumentNullException(nameof(errorName), "Field is required"),
        "argError" => throw new ArgumentException("Field is required"),
        "argInvalidError" => throw new InvalidArgumentException("Field is required"),
        "validationError" => throw new ValidationException(
            new ValidationResult("Field is required", new[] { "FirstName", "LastName" }), null, "Test"),
        "appError" => throw new ApplicationException("System cannot process this request"),
        "dbError" => throw new DbUpdateException("An error occurs in database"),
        "aggregateError" => throw new AggregateException("System cannot process this request", new Exception[]
        {
            new("Inner error"),
            new ArgumentNullException(nameof(errorName), "Field is required")
        }),
        "refitError1" => throw await ApiException.Create("System cannot process this request",
            new HttpRequestMessage(HttpMethod.Get, "Problems"),
            HttpMethod.Get,
            new HttpResponseMessage(HttpStatusCode.BadRequest),
            new RefitSettings()),
        "refitError2" => throw await ApiException.Create(
            "",
            new HttpRequestMessage(HttpMethod.Get, "Problems"),
            HttpMethod.Get,
            new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = JsonContent.Create(new
                {
                    status = 401, traceId = "43245dabc750f944a592802a8edfefb5",
                    errorCode = "Unauthorized",
                    errorMessage = "One or more validation errors occurred."
                })
            },new RefitSettings()),
        "refitError3" => throw await ApiException.Create(
            "",
            new HttpRequestMessage(HttpMethod.Get, "Problems"),
            HttpMethod.Get,
            new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = JsonContent.Create(new
                {
                    error = "Unauthorized",
                    message = "One or more validation errors occurred."
                })
            },new RefitSettings()),
        _ => throw new ArgumentOutOfRangeException(nameof(errorName), errorName, "Out of range")
    };

    [HttpPost("bizError")]
    public ActionResult ThrowBizError()
    {
        IStatusGenericHandler statusGenericHandler = new StatusGenericHandler();
        statusGenericHandler.AddError("Field is required.", "first_name");
        return statusGenericHandler.Send();
    }
}