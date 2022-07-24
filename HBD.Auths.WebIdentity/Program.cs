using HBD.Auths.WebIdentity.Auth.Infra;
using HBDStack.Web.Auths.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMediatR(typeof(Program).Assembly)
    .AddIdentityAuth<AuthDbContext>(new MsIdentityOptions
{
    DbContextOptions = b => { b.UseInMemoryDatabase(nameof(AuthDbContext));}
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

app.UseHttpsRedirection();

app.UseAuthorization()
    .UseAuthentication();

app.MapControllers();

app.Run();