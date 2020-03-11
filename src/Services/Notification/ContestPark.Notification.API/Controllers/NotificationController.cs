using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContestPark.Notification.API.Controllers
{
    public class NotificationController : Core.Controllers.ControllerBase
    {
        #region Constructor

        public NotificationController(ILogger<NotificationController> logger) : base(logger)
        {
        }

        #endregion Constructor

        #region Methods

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                test = "Başarılı!"
            });
        }

        #endregion Methods
    }
}
