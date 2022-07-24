using HBDStack.AspNetCore.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace HBD.AspNetCore.Tests.Controllers;

[Produces("application/json")]
[Route("[controller]")]
public class RateLimitsController : ControllerBase
{
    #region Methods

    [RateLimit(nameof(RateLimitsController) + nameof(Get), 2)]
    public string Get() => "Hello";

    [RateLimit(nameof(RateLimitsController) + nameof(GetById), 2, TrackSuccessfulOnly = true)]
    [HttpGet("Id")]
    public ActionResult GetById() => BadRequest();

    #endregion Methods
}