using Microsoft.Extensions.Logging;

namespace ContestPark.Chat.API.Controllers
{
    public class ChatController : Core.Controllers.ControllerBase
    {
        #region Constructor

        public ChatController(ILogger<ChatController> logger) : base(logger)
        {
        }

        #endregion Constructor
    }
}