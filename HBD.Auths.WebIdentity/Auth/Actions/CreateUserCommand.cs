using System.Text;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace HBD.Auths.WebIdentity.Auth.Actions;

[AutoMap(typeof(IdentityUser))]
public class CreateUserCommand : IRequest<IResult<CreateUserResult>>
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool RequireEmailConfirmation { get; set; }
}

public record CreateUserResult(string Id, string? EmailVerificationCode = null);

internal class CreateUserHandler : IRequestHandler<CreateUserCommand, IResult<CreateUserResult>?>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMediator _mediator;
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(UserManager<IdentityUser> userManager, IMediator mediator, ILogger<CreateUserHandler> logger)
    {
        _userManager = userManager;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IResult<CreateUserResult>?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new IdentityUser { UserName = request.UserName, Email = request.Email };
        var rs = await _userManager.CreateAsync(user, request.Password);

        if (!rs.Succeeded)
            return Result.Fails<CreateUserResult>(rs.Errors);

        _logger.LogInformation("User created {Name} a new account with password", user.Email);

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        if (request.RequireEmailConfirmation)
            return Result.Success(new CreateUserResult(userId, code));

        _logger.LogInformation("Auto confirmed User email {Email}", user.Email);
        await _mediator.Send(new EmailVerificationCommand { UserName = user.UserName, Code = code }, cancellationToken);

        return Result.Success(new CreateUserResult(userId));
    }
}