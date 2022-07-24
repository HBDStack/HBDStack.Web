using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HBD.Auths.WebIdentity.Auth.Actions;

public class EmailVerificationCommand : IRequest<IdentityResult>
{
    public string UserName { get; set; } = default!;
    public string Code { get; set; } = default!;
}

internal class EmailVerificationHandler : IRequestHandler<EmailVerificationCommand, IdentityResult?>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<EmailVerificationHandler> _logger;

    public EmailVerificationHandler(UserManager<IdentityUser> userManager, ILogger<EmailVerificationHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IdentityResult?> Handle(EmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        return await _userManager.ConfirmEmailAsync(user, request.Code);
    }
}