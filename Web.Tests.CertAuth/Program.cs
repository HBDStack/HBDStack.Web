using System.Security.Cryptography.X509Certificates;
using HBDStack.Web.Auths.CertAuth;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Https;

var builder = WebApplication.CreateBuilder(args);

//Setup Cert Auth
builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertAuth(builder.Configuration,
    new CertAuthConfig
    {
        ClientCertificateMode = ClientCertificateMode.AllowCertificate,
        ConfigureOptions = o =>
        {
            // o.AllowedCertificateTypes = CertificateTypes.Chained;
            // o.RevocationFlag = X509RevocationFlag.ExcludeRoot;
            // o.RevocationMode = X509RevocationMode.NoCheck;
            // o.ValidateCertificateUse = false;
            // o.ValidateValidityPeriod = false;
        },
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//Use Cert Auth
app.UseCertAuth();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();