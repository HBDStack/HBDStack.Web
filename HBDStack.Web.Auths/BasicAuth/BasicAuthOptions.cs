namespace HBDStack.Web.Auths.BasicAuth;

public class BasicAuthAccount
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class BasicAuthOptions
{
    public static string Name => "Authentication:Basic";

    public List<BasicAuthAccount> Accounts { get; set; } = new();
}