using Microsoft.Extensions.Logging;

namespace ContestPark.Post.API.Controllers
{
    public class PostController : Core.Controllers.ControllerBase
    {
        #region Constructor

        public PostController(ILogger<PostController> logger) : base(logger)
        {
        }

        #endregion Constructor
    }
}