using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBD.Auths.WebIdentity.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class InfoController : ControllerBase
{
    [HttpGet("Tokens")]
    public async Task<ActionResult> GetToken()
    {
        var name = User.Identity?.Name;
        var roles = User.FindAll(c => c.Type == ClaimTypes.Role).Select(c=>c.Value).Distinct().ToArray();
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var idToken = await HttpContext.GetTokenAsync("id_token");
        return Ok(new {name, roles, idToken, accessToken });
    }
}