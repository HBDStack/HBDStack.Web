using HBD.Auths.WebIdentity.Auth.Infra;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HBD.Auths.WebIdentity.Auth.Queries;

public class UserQuery:IRequest<IEnumerable<UserResult>>
{
    
}

public record UserResult(string Id, string UserName);

internal class UserQueryHandler : IRequestHandler<UserQuery,IEnumerable<UserResult>>
{
    private readonly AuthDbContext _context;

    public UserQueryHandler(AuthDbContext context) => _context = context;

    public async Task<IEnumerable<UserResult>> Handle(UserQuery request, CancellationToken cancellationToken)
        => await _context.Set<IdentityUser>().Select(u => new UserResult(u.Id, u.UserName)).ToListAsync(cancellationToken: cancellationToken);
}