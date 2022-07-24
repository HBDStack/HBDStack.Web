using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HBD.Auths.WebIdentity.Auth.Actions;

public class CreateRoleCommand : IRequest<IResult<RoleResult>>
{
    public string Name { get; set; } = default!;
}

public record RoleResult(string Id, string Name);

internal class CreateRoleHandler : IRequestHandler<CreateRoleCommand, IResult<RoleResult>>
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public CreateRoleHandler(RoleManager<IdentityRole> roleManager) => _roleManager = roleManager;

    public async Task<IResult<RoleResult>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var identityRole = await _roleManager.FindByNameAsync(request.Name);

        if (identityRole == null)
        {
            identityRole = new IdentityRole { Name = request.Name };
            
            var rs = await _roleManager.CreateAsync(identityRole);
            if (!rs.Succeeded) Result.Fails<RoleResult>(rs.Errors);
            
            await _roleManager.AddClaimAsync(identityRole, new Claim(ClaimTypes.Role, request.Name));
        }

        return Result.Success(new RoleResult(identityRole.Id, identityRole.Name));
    }
}