using HBDStack.Web.Auths;
using HBDStack.Web.Auths.CertAuth;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.IdentityModel.Logging;
using Web.Tests.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwagger();
    app.UseSwaggerUI();
    IdentityModelEventSource.ShowPII = true;
}

app.UseHttpsRedirection();

app.UseCertAuth();

//app.UseAuth();

app.MapControllers();

app.UseGlobalExceptionHandler<CustomGlobalExceptionHandler>();

app.Run();

namespace Web.Tests
{
    public partial class Program
    {
    }
}