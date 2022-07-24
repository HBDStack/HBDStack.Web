using HBDStack.Web.Auths.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class AuthSetup
{
    /// <summary>
    /// This method will config cookie as well no need to add Cookie authentication anymore
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    /// <returns></returns>
    public static IdentityBuilder AddIdentityAuth<TUser,TRole, TDbContext>(this IServiceCollection services,  MsIdentityOptions options) where TUser : class where TRole : class where TDbContext : DbContext
    {
        services.AddDbContext<TDbContext>(b=>options.DbContextOptions?.Invoke(b));
        
        var identityBuilder = services
            .AddIdentity<TUser, TRole>(o =>
            {
                // Password settings.
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequireUppercase = true;
                o.Password.RequiredLength = 8;
                o.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                o.Lockout.MaxFailedAccessAttempts = 5;
                o.Lockout.AllowedForNewUsers = true;

                // User settings.
                // options.User.AllowedUserNameCharacters =
                //     "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                o.User.RequireUniqueEmail = true;

                options.Options?.Invoke(o);
            })
            .AddEntityFrameworkStores<TDbContext>()
            .AddDefaultTokenProviders();

        //This is require when ProtectPersonalData = true
        // builder.Services.AddScoped<ILookupProtectorKeyRing, KeyRing>();
        // builder.Services.AddScoped<ILookupProtector, LookupProtector>();
        // builder.Services.AddScoped<IPersonalDataProtector, PersonalDataProtector>();
        
        services.ConfigureApplicationCookie(o=>
        {
            // Cookie settings
            o.Cookie.HttpOnly = true;
            o.Cookie.SameSite = SameSiteMode.Lax;
            o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            o.ExpireTimeSpan = TimeSpan.FromMinutes(5);

            o.LoginPath = "/Identity/Account/Login";
            o.AccessDeniedPath = "/Identity/Account/AccessDenied";
            o.SlidingExpiration = true;
            
            options.CookieOptions?.Invoke(o);
        });
        
        return identityBuilder;
    }

    public static IdentityBuilder AddIdentityAuth<TUser, TDbContext>(this IServiceCollection services, MsIdentityOptions options) where TUser : class where TDbContext : DbContext
        => services.AddIdentityAuth<TUser, IdentityRole, TDbContext>(options);
    
    public static IdentityBuilder AddIdentityAuth<TDbContext>(this IServiceCollection services, MsIdentityOptions options) where TDbContext : DbContext
        => services.AddIdentityAuth<IdentityUser, TDbContext>(options);
}