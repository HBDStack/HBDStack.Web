using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApiTest.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult Get() => Ok();

    [HttpGet("{id}")]
    public ActionResult Get(string id) => Ok(id);

    [HttpGet("{id}:find")]
    [SwaggerOperation(OperationId = "Custom_WeatherForecast_GetById")]
    public ActionResult GetById(string id) => Ok(id);
}