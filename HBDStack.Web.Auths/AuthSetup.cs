using HBDStack.Web.Auths;
using HBDStack.Web.Auths.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class AuthSetup
{
    public static IServiceCollection AddClaimsProvider<TClaimProvider>(this IServiceCollection services)
        where TClaimProvider : class, IClaimsProvider
        => services.AddSingleton<IClaimsProvider, TClaimProvider>();

    public static IServiceCollection AddTokenValidator<TTokenValidator>(this IServiceCollection services)
        where TTokenValidator : class, ITokenValidator
        => services.AddSingleton<ITokenValidator, TTokenValidator>();
    
    /// <summary>
    /// Ad Multi Authentications. You need to define the list of schemas for each Auth Provider.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="defaultScheme"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddAuth(this IServiceCollection services, AuthsOptions? options=null, string? defaultScheme = null)
    {
        //Ensure options is not null
        options ??= new AuthsOptions();
        
        services.AddAuthorization(op =>
        {
            if (!string.IsNullOrWhiteSpace(defaultScheme))
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(defaultScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            
                op.AddPolicy($"{defaultScheme}-Policy", policy);
            }

            op.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            //Add more policy from options
            options.DefaultAuthPolicyBuilder?.Invoke(op);
        });

        var builder = string.IsNullOrWhiteSpace(defaultScheme) ? services.AddAuthentication() : services.AddAuthentication(op =>
        {
            op.DefaultScheme = defaultScheme;
            op.DefaultChallengeScheme = defaultScheme;
            options.AuthenticationOptionsBuilder?.Invoke(op);
        });

        if (options.DefaultSchemeForwarder != null && !string.IsNullOrWhiteSpace(defaultScheme))
            builder.AddPolicyScheme(defaultScheme, "Switching between schemes",
                op => op.ForwardDefaultSelector = options.DefaultSchemeForwarder);

        return builder;
    }

    /// <summary>
    /// Call this in your Application configuration setup
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication()
            .UseAuthorization();

        return app;
    }
}