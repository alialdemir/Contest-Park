using Microsoft.Extensions.Logging;

namespace ContestPark.Admin.API.Controllers
{
    public class SubCategoryController : Core.Controllers.ControllerBase
    {
        #region Constructor

        public SubCategoryController(ILogger<SubCategoryController> logger) : base(logger)
        {
        }

        #endregion Constructor
    }
}
