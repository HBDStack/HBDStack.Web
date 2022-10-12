using HBDStack.Web.Swagger;
using Microsoft.IdentityModel.Logging;
using Web.Tests.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddApiExplorer();
builder.Services.AddSwaggerConfig(new SwaggerInfo{Title = "Test Api"});

// builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
//     .AddCertAuth<CertAuthHandler>(builder.Configuration,
//     new CertAuthConfig
//     {
//         ClientCertificateMode = ClientCertificateMode.RequireCertificate,
//         ConfigureOptions = o =>
//         {
//             o.AllowedCertificateTypes = CertificateTypes.All;
//             o.ValidateCertificateUse = false;
//             o.ValidateValidityPeriod = false;
//         },
//     });

// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookieAuth();

// builder.Services
//     .AddClaimsProvider<ClaimsProvider>()
//     .AddAuth(new AuthsOptions
//     {
//         DefaultSchemeForwarder = context =>
//         {
//             context.Request.Headers.TryGetValue("x-auth-provider", out var provider);
//             return provider;
//         }
//     }, "WebTest")
//     .AddJwtAuths(builder.Configuration);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerAndUI();
    IdentityModelEventSource.ShowPII = true;
}

app.UseHttpsRedirection();

//app.UseCertAuth();

app.UseAuth();

app.MapControllers();

app.UseGlobalExceptionHandler<CustomGlobalExceptionHandler>();

app.Run();

namespace Web.Tests
{
    public partial class Program
    {
    }
}