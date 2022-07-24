using HBDStack.AspNetCore.Extensions.JwtAuth;
using HBDStack.AspNetCore.Extensions.Providers;
using HBDStack.AzProxy.Core.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using HBDStack.AspNetCore.Extensions.Internals;
using Refit;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class AuthExtensions
{
    #region Methods

    /// <summary>
    /// Add Single Authentication with custom Schema.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="schema">The schema name of authentication provider.</param>
    /// <param name="authOptions">To add more custom policies, using this to add more option for each schema.</param>
    /// <returns></returns>
    public static AuthenticationBuilder AddAuth(this IServiceCollection services, string schema, Action<AuthorizationOptions> authOptions = null)
        => services.AddAuthorizationPolicy(schema, (s, o) => { authOptions?.Invoke(o); })
            .AddAuthentication(schema);

    /// <summary>
    /// Add AzAdGroupClaimsProvider
    /// </summary>
    /// <param name="services"></param>
    /// <param name="validator"></param>
    /// <returns></returns>
    public static IServiceCollection AddAzAdGroupClaimsProvider(this IServiceCollection services, AzAdGroupClaimsValidator validator)
        => services.AddSingleton(validator)
            .AddClaimsProvider<AzAdGroupClaimsProvider>();

    /// <summary>
    /// Add an implementation of  IClaimsProvider<TOptions>
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TProvider"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddClaimsProvider<TProvider, TOptions>(this IServiceCollection services)
        where TProvider : class, IClaimsProvider<TOptions>
        where TOptions : AuthenticationSchemeOptions
        => services.AddSingleton<IClaimsProvider<TOptions>, TProvider>();

    /// <summary>
    /// Add an implementation of IJwtClaimsProvider
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddClaimsProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IJwtClaimsProvider
        => services.AddSingleton<IClaimsProvider<JwtBearerOptions>, TProvider>();

    /// <summary>
    /// Add JTW Beare authentication
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="appName"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration, string appName = null,
        Action<AuthorizationOptions> authOptions = null)
        => services.AddJwtAuth<DefaultJwtBearerEvents>(configuration, appName, authOptions);

    /// <summary>
    /// Add JTW Beare authentication
    /// </summary>
    /// <typeparam name="TEvents"></typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="appName"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwtAuth<TEvents>(this IServiceCollection services, IConfiguration configuration, string appName = null,
        Action<AuthorizationOptions> authOptions = null) where TEvents : DefaultJwtBearerEvents
    {
        var setting = configuration.Bind<JwtAuthSetting>(JwtAuthSetting.Name);

        if (setting.Apps.Count <= 0)
            throw new ArgumentException($"There is no app found in {JwtAuthSetting.Name} setting.");
        if (setting.Apps.Count > 1 && string.IsNullOrEmpty(appName))
            throw new ArgumentNullException($"There more than 1 app found. Please specify the {nameof(appName)}.");

        var app = setting.Apps.Single(a => a.Name.Equals(appName, StringComparison.OrdinalIgnoreCase));
        return services.AddJwtAuth<TEvents>(app, authOptions);
    }

    /// <summary>
    /// Add JTW Beare authentication
    /// </summary>
    /// <param name="services"></param>
    /// <param name="appSetting"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, JwtAppAuth appSetting, Action<AuthorizationOptions> authOptions = null)
        => services.AddJwtAuth<DefaultJwtBearerEvents>(appSetting, authOptions);

    /// <summary>
    /// Add JTW Beare authentication
    /// </summary>
    /// <typeparam name="TEvents"></typeparam>
    /// <param name="services"></param>
    /// <param name="appSetting"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwtAuth<TEvents>(this IServiceCollection services, JwtAppAuth appSetting, Action<AuthorizationOptions> authOptions = null)
        where TEvents : DefaultJwtBearerEvents
    {
        services
            .AddAuth(JwtBearerDefaults.AuthenticationScheme, authOptions)
            .AddBearerAuth<TEvents>(appSetting);

        return services;
    }

    /// <summary>
    /// This provider will add Authentication and graph_token to Claims. For Service to Service calling.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwtTokenClaimsProvider(this IServiceCollection services)
    {
        services
            .AddRefitClient<IGraphClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://graph.microsoft.com/v1.0"));
            
        return services
            .AddClaimsProvider<JwtTokenClaimsProvider>()
            .AddScoped<IAuthTokenHeaderProvider, JwtTokenHeaderProvider>();
    }

    /// <summary>
    /// Ad Multi Authentications. You need to define the list of schemas for each Auth Provider.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="schemas"></param>
    /// <param name="forwardDefaultSelector">This is required to deside which schema should be pickedup.</param>
    /// <param name="authOptions">To add more custom policies, using this to add more option for each schema.</param>
    /// <returns></returns>
    public static AuthenticationBuilder AddMultiAuths(this IServiceCollection services, string[] schemas, Func<HttpContext, string> forwardDefaultSelector,
        Action<string, AuthorizationOptions> authOptions = null)
    {
        var schemaName = string.Join("-", schemas);

        var builder = services.AddAuthorizationPolicy(schemaName, authOptions)
            .AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = schemaName;
                sharedOptions.DefaultChallengeScheme = schemaName;
            }).AddPolicyScheme(schemaName, "Switching between schema", options => { options.ForwardDefaultSelector = forwardDefaultSelector; });

        return builder;
    }

    /// <summary>
    /// Get Decode Jwt Token from `Authorization` Header.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static HBDStack.AzProxy.Core.JwtToken GetJwtToken(this HttpContext context)
    {
        context.Request.Headers.TryGetValue("Authorization", out var token);

        if (string.IsNullOrEmpty(token)) return null;

        var stoken = token.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        return HBDStack.AzProxy.Core.JwtDecode.Decode(stoken);
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

    private static IServiceCollection AddAuthorizationPolicy(this IServiceCollection services, string schema, Action<string, AuthorizationOptions> authOptions)
        => services.AddAuthorization(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(schema)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy($"{schema}-Policy", policy);

            //Add custion options
            authOptions?.Invoke(schema, options);
        });

    #endregion Methods
}