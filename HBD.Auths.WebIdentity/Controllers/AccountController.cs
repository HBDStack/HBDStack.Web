using HBD.Auths.WebIdentity.Auth.Actions;
using HBD.Auths.WebIdentity.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBD.Auths.WebIdentity.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class AccountsController: ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IEnumerable<UserResult>> Get([FromQuery]UserQuery query) => await _mediator.Send(query);
    
    [HttpPost]
    public async Task<IResult<CreateUserResult>> CreateUser(CreateUserCommand command) => await _mediator.Send(command);
}