using Microsoft.AspNetCore.Mvc;

namespace ContestPark.Core.Controllers
{
    [ApiController]
    [Route("hc")]
    [Produces("application/json")]
    public class HealthCheckController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
