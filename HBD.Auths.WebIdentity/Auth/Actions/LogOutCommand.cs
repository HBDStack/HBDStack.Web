using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HBD.Auths.WebIdentity.Auth.Actions;

public class LogOutCommand:IRequest
{
    public string UserName { get; set; } = default!;
}

internal class LogOutHandler : IRequestHandler<LogOutCommand>
{
    private readonly SignInManager<IdentityUser> _signInManager;
    public LogOutHandler(SignInManager<IdentityUser> signInManager) => _signInManager = signInManager;

    public async Task<Unit> Handle(LogOutCommand request, CancellationToken cancellationToken)
    {
      await  _signInManager.SignOutAsync();
      return Unit.Value;
    }
}