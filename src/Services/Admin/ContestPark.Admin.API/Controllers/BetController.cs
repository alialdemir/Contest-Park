using Amazon.Runtime.Internal.Util;
using ContestPark.Admin.API.Infrastructure.Repositories.Bet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class BetController : Core.Controllers.ControllerBase
    {
        #region Constructor

        public BetController(ILogger<BetController> logger,
            IBetRepository betRepository) : base(logger)
        {
        }

        #endregion Constructor
    }
}
