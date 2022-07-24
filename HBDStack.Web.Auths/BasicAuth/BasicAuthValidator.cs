using Microsoft.Extensions.Options;

namespace HBDStack.Web.Auths.BasicAuth;

public interface IBasicAuthValidator
{
    (bool success, string? error) Validate(string? userName, string? password);
}

public class BasicAuthValidator : IBasicAuthValidator
{
    private readonly BasicAuthOptions _basicAuthOptions;

    public BasicAuthValidator(IOptions<BasicAuthOptions> basicAuthOptions) => _basicAuthOptions = basicAuthOptions.Value;

    public (bool success, string? error) Validate(string? userName, string? password)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)
                                           || !_basicAuthOptions.Accounts.Any(a => a.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && a.Password == password))
            return (false, "Invalid Username or Password");
        return (true, null);
    }
}