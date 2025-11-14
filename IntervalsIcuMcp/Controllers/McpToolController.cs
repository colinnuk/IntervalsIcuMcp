using Microsoft.AspNetCore.Mvc;

namespace IntervalsIcuMcp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class McpToolController : ControllerBase
    {
        [HttpGet(Name = "GetWeatherForecast")]
        public string Get()
        {
            return string.Empty;
        }
    }
}
