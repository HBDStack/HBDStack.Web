using HBDStack.Web.RequestLogs;
using HBDStack.Web.RequestLogs.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Logs.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class LogsController : ControllerBase
{
    [HttpGet("Guids")]
    public IEnumerable<Guid> GetGuids([FromQuery]string? name) => Enumerable.Range(1, 100).Select(i => Guid.NewGuid());
    
    [HttpGet]
    public async Task<ActionResult> Get([FromServices]IRequestLogService service)
    {
        var list = new List<LogItem>();
       await foreach (var item in service.GetLogsAsyncEnumerable())
           list.Add( item);

       return Ok(new {Count=list.Count,List=list});
    }
}