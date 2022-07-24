using System.ComponentModel.DataAnnotations;
using HBDStack.StatusGeneric;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Tests.Controllers;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class ExceptionsController : ControllerBase
{
    private readonly ILogger<ExceptionsController> _logger;

    public ExceptionsController(ILogger<ExceptionsController> logger) => _logger = logger;

    [HttpGet("Exception")]
    public ActionResult Get() => throw new InvalidOperationException("Invalid operation");

    [HttpGet("Status")]
    public ActionResult GetStatus()
    {
        var status = new StatusGenericHandler();
        
        status.AddError("Some error");
        status.AddValidationResult(new ValidationResult("Validation error", new List<string> { "Error" }));
        status.AddValidationResult(new GenericValidationResult("errorCode", "Validation error", new [] { "Error1" }));

        return status.Send();
    }
}