using HBD.Auths.WebIdentity.Auth.Actions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBD.Auths.WebIdentity.Controllers;

[ApiController]
[Route("auth/[controller]")]
public class AuthController: ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost(nameof(Login))]
    [AllowAnonymous]
    public Task<IResult<LoginResult>> Login(LoginCommand command) => _mediator.Send(command);
    
    [HttpPost(nameof(LogOut))]
    public Task LogOut() => _mediator.Send(new LogOutCommand{UserName = User.Identity!.Name!});
}