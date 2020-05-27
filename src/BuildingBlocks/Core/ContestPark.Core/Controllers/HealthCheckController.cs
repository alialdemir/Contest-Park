using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ContestPark.Core.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class HealthCheckController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Get()
        {
            return Ok("ok");
        }
    }
}
