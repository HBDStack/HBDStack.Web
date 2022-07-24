using HBDStack.Web.Auths.CookieAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CookieAuthSetup
{
    public static AuthenticationBuilder AddCookieAuth(this AuthenticationBuilder services, CookieAuthConfig? config = null)
        => services.AddCookieAuth<DefaultCookieEvents>(config);

    public static AuthenticationBuilder AddCookieAuth<TEvents>(this AuthenticationBuilder services, CookieAuthConfig? config = null, string scheme = CookieAuthenticationDefaults.AuthenticationScheme) where TEvents : DefaultCookieEvents
    {
        config ??= new CookieAuthConfig();

        services.Services.TryAddScoped<TEvents>();

        if (config.IncludesLaxPolicy)
            services.Services.AddLaxCookiePolicy();

        void Options(CookieAuthenticationOptions op)
        {
            op.Cookie.SameSite = SameSiteMode.Lax;
            op.Cookie.HttpOnly = true;
            op.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            op.Cookie.IsEssential = true;
            
            config.Options?.Invoke(op);
            op.EventsType = typeof(TEvents);
        }

        return services.AddCookie(scheme, Options);
    }

    private static IServiceCollection AddLaxCookiePolicy(this IServiceCollection services)
        => services.Configure<CookiePolicyOptions>(op =>
        {
            op.MinimumSameSitePolicy = SameSiteMode.Lax;
            op.Secure = CookieSecurePolicy.SameAsRequest;
            op.HttpOnly = AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
        });
}