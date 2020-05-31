using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ContestPark.Core.Controllers
{
    [ApiController]
    [Route("hc")]
    [Produces("application/json")]
    public class HealthCheckController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult Get()
        {
            return Ok("ok");
        }
    }
}
