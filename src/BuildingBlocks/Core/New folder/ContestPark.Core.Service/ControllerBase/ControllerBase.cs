using ContestPark.Core.Enums;
using ContestPark.Core.Service.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace ContestPark.Core.Service
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        #region Constructor

        public ControllerBase(
            ILogger<ControllerBase> logger
            )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Constructor

        #region Properties

        public ILogger<ControllerBase> Logger { get; private set; }

        // <summary>
        // Current user language
        // </summary>
        public Languages CurrentUserLanguage
        {
            get
            {
                if (Request.Headers["Accept-Language"].Count == 0)
                    return Languages.English;

                string langCode = Request.Headers["Accept-Language"].ToString();
                if (langCode == "tr-TR") return Languages.Turkish;

                return Languages.English;
            }
        }

        private string userId = String.Empty;

        /// <summary>
        /// Current user id
        /// </summary>
        public string UserId
        {
            get
            {
                if (String.IsNullOrEmpty(userId))
                {
                    userId = User.FindFirst("sub")?.Value;
                }

                if (String.IsNullOrEmpty(userId))
                {
                    Logger.LogWarning($"{nameof(UserId)} is empty.");
                }

                return userId;
            }
        }

        private string userFullName;

        /// <summary>
        /// Current user full name
        /// </summary>
        public string UserFullName
        {
            get
            {
                if (String.IsNullOrEmpty(userFullName)) userFullName = User.FindFirst("fullName")?.Value;
                return userFullName;
            }
        }

        #endregion Properties

        #region Methods

        public override BadRequestObjectResult BadRequest(object error)
        {
            return base.BadRequest(new MessageModel(error.ToString()));
        }

        #endregion Methods
    }
}