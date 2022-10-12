using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Tests.Controllers;

[Authorize]
[ApiController]
[ApiVersion("2.0")]
[Route("v{version:apiVersion}/[controller]")]
public class RolesController : ControllerBase
{
    [HttpGet]
    public ActionResult Get() =>
        Ok(User.FindAll(c => c.Type == ClaimTypes.Role).Select(c => c.Value));
    
    [HttpGet("Claims")]
    public ActionResult GetClaims() =>
        Ok(User.Claims.Select(c =>new{c.Type, c.Value}));
}