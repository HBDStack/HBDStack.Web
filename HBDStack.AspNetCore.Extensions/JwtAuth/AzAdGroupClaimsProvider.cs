using HBDStack.AspNetCore.Extensions.Internals;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.JsonWebTokens;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

// ReSharper disable ClassNeverInstantiated.Global

namespace HBDStack.AspNetCore.Extensions.JwtAuth;

/// <summary>
/// This Provider will call Microsoft Graph API and convert Groups in to Roles
/// </summary>
public class AzAdGroupClaimsProvider : IJwtClaimsProvider
{
    private readonly AzAdGroupClaimsValidator _validator;
    private readonly IDistributedCache _distributedCache;
    private readonly IGraphClient _graphClient;

    public AzAdGroupClaimsProvider(AzAdGroupClaimsValidator validator, IGraphClient graphClient, [AllowNull] IEnumerable<IDistributedCache> distributedCaches = null)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _distributedCache = distributedCaches?.FirstOrDefault();
        _graphClient = graphClient ?? throw new ArgumentNullException(nameof(graphClient));
    }

    #region Properties

    /// <summary>
    /// The cache expiration period
    /// </summary>
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromHours(4);

    /// <summary>
    /// The name of Identity claim by default it will find the claims from JwtRegisteredClaimNames.UniqueName or JwtRegisteredClaimNames.Sid or JwtRegisteredClaimNames.Email
    /// </summary>
    public string IdentityClaimName { get; set; }

    #endregion Properties

    #region Methods

    public async Task<ICollection<Claim>> GetClaimsAsync(ResultContext<JwtBearerOptions> context)
    {
        if (context != null && !string.IsNullOrEmpty(_validator.Scheme) && context.Scheme.Name != _validator.Scheme)
        {
            Trace.TraceInformation($"AzAdGroupClaimsProvider was ignored as the Scheme {_validator.Scheme} and {context.Scheme.Name} are matched:");
            return Array.Empty<Claim>();
        }

        var token = GetTokens(context as TokenValidatedContext);
        if (token == null)
            return Array.Empty<Claim>();

        var groups = await GetGroupsFromCache(context as TokenValidatedContext, token);

        var claims = groups.Where(g => g.SecurityEnabled == true && _validator.ValidGroup(g.DisplayName)).Select(g => _validator.MapGroupToClaim(g.DisplayName));
        return claims.ToList();
    }


    /// <summary>
    /// Delegating method allows to overwrite and mock. Default will call <see cref="JwtTokenClaimsProvider.GetTokens"/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual Tokens GetTokens(TokenValidatedContext context) => JwtTokenClaimsProvider.GetTokens(context);

    /// <summary>
    /// Get Group Info from Cookie pr Graph
    /// </summary>
    /// <param name="context"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    private async Task<IEnumerable<GroupData>> GetGroupsFromCache(TokenValidatedContext context, Tokens tokens)
    {
        var principal = context.Principal ?? context.HttpContext.User;

        var identicalName = string.IsNullOrEmpty(IdentityClaimName)
            ? principal?.Claims.FirstOrDefault(c => c.Type is JwtRegisteredClaimNames.UniqueName or JwtRegisteredClaimNames.Sid or JwtRegisteredClaimNames.Email)
            : principal?.Claims.FirstOrDefault(c => c.Type == IdentityClaimName);

        if (identicalName == null) throw new UnauthorizedAccessException(nameof(IdentityClaimName));

        var key = $"{nameof(AzAdGroupClaimsProvider)}.{context.Scheme.Name}.{identicalName.Value}";
        var strVal = _distributedCache != null ? await _distributedCache.GetStringAsync(key) : null;

        GroupData[] groups;

        if (!string.IsNullOrEmpty(strVal))
            groups = System.Text.Json.JsonSerializer.Deserialize<GroupData[]>(strVal);
        else
        {
            groups = (await GetGroupsFromGraph(tokens)).ToArray();

            if (groups.Any())
            {
                if (_distributedCache == null)
                    Trace.TraceWarning($"${nameof(AzAdGroupClaimsProvider)}:{nameof(IDistributedCache)} is not provided.");
                else
                    await _distributedCache.SetStringAsync(key, System.Text.Json.JsonSerializer.Serialize(groups),
                        new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = CacheExpiration});
            }
        }

        return groups;
    }

    /// <summary>
    /// Get Group Info from Graph
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    protected virtual async Task<IEnumerable<GroupData>> GetGroupsFromGraph(Tokens tokens)
    {
        string token;

        if (!string.IsNullOrEmpty(tokens.GraphToken))
        {
            token = tokens.GraphToken;
            Trace.TraceInformation($"Try get groups with {nameof(tokens.GraphToken)}.");
        }
        else
        {
            token = tokens.AccessToken;
            Trace.TraceInformation($"Try get groups with {nameof(tokens.AccessToken)}.");
        }

        if (string.IsNullOrEmpty(token)) return Enumerable.Empty<GroupData>();

        try
        {
            //Get Groups from Azure AD.
            var groups = await _graphClient.GetGroups(token);
            return groups.Value;
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Failed to get the groups with provided token with error '{ex.Message}'.");
        }

        return Enumerable.Empty<GroupData>();
    }

    #endregion Methods
}

public sealed class Tokens
{
    #region Properties

    public string AccessToken { get; set; }

    public string GraphToken { get; set; }

    #endregion Properties
}