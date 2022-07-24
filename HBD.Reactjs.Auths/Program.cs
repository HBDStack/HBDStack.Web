using HBD.Reactjs.Auths.Providers;
using HBDStack.Web.Auths.BasicAuth;
using HBDStack.Web.Auths.Providers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// builder.Services
//     .AddScoped<IClaimsProvider,ClaimsProvider>()
//     .AddAuth(defaultScheme: CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookieAuth()
//     .AddOpenIdAuths(builder.Configuration)
//     .AddSaml2Auths(builder.Configuration);

builder.Services
    .AddScoped<IClaimsProvider, ClaimsProvider>()
    .AddAuth(defaultScheme: BasicAuthDefaults.Scheme)
    .AddBasicAuth(builder.Configuration);

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen(setup =>
    {
        //setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "HBD.Reactjs.Auths.xml"), true);

        setup.SwaggerDoc("v1", new OpenApiInfo()
        {
            Description = "The API definition",
            Title = "Api",
            Version = "v1",
            Contact = new OpenApiContact()
            {
                Name = "system@drunkcoding.net",
                Url = new Uri("https://www.drunkcoding.net")
            }
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuth();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.UseSwagger().UseSwaggerUI(c =>
{
    c.RoutePrefix = "docs";
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1");
});

app.MapFallbackToFile("index.html");

app.Run();