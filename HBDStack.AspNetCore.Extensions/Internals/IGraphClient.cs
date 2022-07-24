using Refit;

namespace HBDStack.AspNetCore.Extensions.Internals;

public interface IGraphClient
{
    [Get("/me/transitiveMemberOf/microsoft.graph.group")]
    Task<GroupResult> GetGroups([Authorize("Bearer")] string token);
}