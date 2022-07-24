using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HBDStack.Web.Auths;

public static class AspCoreExtensions
{
    public static bool IsAzureAdAuthority(this string authority) => authority.Contains("login.microsoftonline.com");
    
   /// <summary>
    /// Convert Base64String to Cert
    /// </summary>
    /// <param name="base64String"></param>
    /// <returns></returns>
    public static X509Certificate2? ConvertBase64ToCert(string base64String)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return null;

            return new X509Certificate2(Convert.FromBase64String(base64String), (string?)null,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Cannot parse client certificate: ", ex);
        }
    }

    /// <summary>
    /// Get Client Cert from Request.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static X509Certificate2? GetClientCert(this ActionExecutingContext context) => context.HttpContext.Connection.ClientCertificate;

    public static X509Certificate2? GetForwardingCertificate(this ActionExecutingContext context, string headerKey = "X-ARR-ClientCert")
    {
        var value = context.HttpContext.Request.Headers[headerKey];
        return string.IsNullOrEmpty(value) ? null : ConvertBase64ToCert(value);
    }
}