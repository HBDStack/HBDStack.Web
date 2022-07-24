using System.Security.Claims;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace HBDStack.AspNetCore.Extensions.JwtAuth;

public class AzAdGroupClaimsValidator
{
    #region Constructors
        
    public AzAdGroupClaimsValidator(string environment, string roleKey = "ROL", string scheme = null)
    {
        if (string.IsNullOrWhiteSpace(environment))
        {
            throw new ArgumentException("message", nameof(environment));
        }

        Environment = environment;
        RoleKey = roleKey;
        Scheme = scheme;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Environment identifier key of Group Name. The group must contained this key.
    /// </summary>
    public string Environment { get; }

    /// <summary>
    /// Role identifier key of Group Name. The group must contained this key.
    /// </summary>
    public string RoleKey { get; }

    /// <summary>
    /// Allow filter by Authentication Scheme
    /// </summary>
    public string Scheme { get; }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Convert group to Claim. ex "NON PROD ROL CONTRIBUTERS" => return "CONTRIBUTERS" claim.
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public virtual Claim MapGroupToClaim(string groupName)
    {
        var splits = groupName.Split(new[] { ' ', '-' });
        return new Claim(ClaimTypes.Role, splits[^1]);
    }

    /// <summary>
    /// Check whether the group is defiend for Role access.
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public virtual bool ValidGroup(string groupName) => !string.IsNullOrEmpty(groupName) && groupName.Contains(Environment) && groupName.Contains(RoleKey);

    #endregion Methods
}