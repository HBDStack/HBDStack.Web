using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HBD.Auths.WebIdentity.Auth.Actions;

public class LoginCommand:IRequest<IResult<LoginResult>>
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool RememberMe { get; set; }
}

public record LoginResult(string? TwoFaCode=null);

internal class LoginHandler : IRequestHandler<LoginCommand, IResult<LoginResult>>
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public LoginHandler(SignInManager<IdentityUser> signInManager,UserManager<IdentityUser>userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<IResult<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var rs = await _signInManager.PasswordSignInAsync(request.UserName, request.Password,request.RememberMe,true);
        if (!rs.Succeeded) return Result.Fails<LoginResult>(new []{new IdentityError{Code = "Error",Description = "Login failed"}});

        if (!rs.RequiresTwoFactor)
            return Result.Success(new LoginResult());

        var user =await _userManager.FindByNameAsync(request.UserName);
        var code = await _userManager.GenerateTwoFactorTokenAsync(user,_userManager.Options.Tokens.AuthenticatorTokenProvider);
        
        return Result.Success(new LoginResult(code));
    }
}