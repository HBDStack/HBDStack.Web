using HBDStack.Web.RequestLogs.ApiRequests;
using HBDStack.Web.RequestLogs.Storages.Sql.InternalStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//API Logging
builder.Services.AddRequestLogs(new ApiRequestLoggingOptions("ApiLogging")
{
    IncludesPaths = new List<string> {"/Logs/Guids" },
    ExcludesPaths = new List<string> {"/Logs"},
    
    CaptureRequestBody = true,
    CaptureResponseBody = true,
    CaptureClientIpAddress = true
}).AddSqlRequestStorage(new SqlRequestLogOptions(builder.Configuration.GetConnectionString("Db"))
{
    TableName = "ApiLoggings2"
});

var app = builder.Build();
app.UseRequestLogs();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program
{
}