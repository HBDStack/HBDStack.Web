using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HBDStack.AspNetCore.Extensions.CookieAuth;

public static class CookieConfig
{
    #region Methods

    public static AuthenticationBuilder AddCookieAuth(this AuthenticationBuilder services, IWebHostEnvironment env, bool includesLaxPolicy = true)
        => services.AddCookieAuth<DefaultCookieEvents>(env, includesLaxPolicy);

    public static AuthenticationBuilder AddCookieAuth(this AuthenticationBuilder services, string scheme, IWebHostEnvironment env, bool includesLaxPolicy = true)
        => services.AddCookieAuth<DefaultCookieEvents>(scheme, env, includesLaxPolicy);

    public static AuthenticationBuilder AddCookieAuth<TEvents>(this AuthenticationBuilder services, IWebHostEnvironment env, bool includesLaxPolicy = true) where TEvents : DefaultCookieEvents
        => services.AddCookieAuth<TEvents>(CookieAuthenticationDefaults.AuthenticationScheme, env, includesLaxPolicy);

    public static AuthenticationBuilder AddCookieAuth<TEvents>(this AuthenticationBuilder services, string scheme, IWebHostEnvironment env, bool includesLaxPolicy = true) where TEvents : DefaultCookieEvents
    {
        services.Services.AddSingleton<TEvents>();

        if (includesLaxPolicy)
            services.Services.AddLaxCookiePolicy(env);

        return services.AddCookie(scheme, op =>
        {
            op.SlidingExpiration = false;
            op.EventsType = typeof(TEvents);
        });
    }

    /// <summary>
    /// Add Cookie Policy with Lax
    /// </summary>
    /// <param name="services"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    public static IServiceCollection AddLaxCookiePolicy(this IServiceCollection services, IWebHostEnvironment env)
        => services.Configure<CookiePolicyOptions>(op =>
        {
            op.MinimumSameSitePolicy = SameSiteMode.Lax;

            op.Secure = env.IsDevelopment()
                ? CookieSecurePolicy.None
                : CookieSecurePolicy.Always;

            op.HttpOnly = env.IsDevelopment()
                ? Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.None
                : Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
        });

    #endregion Methods
}