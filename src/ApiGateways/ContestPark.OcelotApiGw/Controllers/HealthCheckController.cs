using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ContestPark.OcelotApiGw.Controllers
{
    [ApiController]
    [Route("hc")]
    [Produces("application/json")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult Get()
        {
            return Ok("ok");
        }
    }
}
