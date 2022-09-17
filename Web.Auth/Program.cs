using HBDStack.Web.Auths;
using Web.Auth.Configs;
using Web.Auth.Configs.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole().AddDebug();

var feature = builder.Configuration.Bind<FeatureOptions>(FeatureOptions.Name);

// Add services to the container.
if (feature.EnableJwtAuth)
{
    builder.Services.AddAuth(new AuthsOptions
        {
            DefaultSchemeForwarder = ctx =>
            {
                ctx.Request.Headers.TryGetValue("xs-auth-schema", out var value);
                return value;
            }
        },defaultScheme:"Web.Auths")
        .AddJwtAuths<JwtAuthHandler>(builder.Configuration);
}

builder.Services.AddHttpClient()
    .AddHttpContextAccessor()
    .AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.ConfigureExceptionHandler();
app.UseHttpsRedirection();

app.UseAuth();

app.MapControllers()
    .RequireAuthorization();

app.Run();


//This Startup endpoint for Unit Tests
namespace Web.Auth
{
    public partial class Program
    {
    }
}