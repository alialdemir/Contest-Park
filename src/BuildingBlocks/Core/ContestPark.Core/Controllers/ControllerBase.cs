using ContestPark.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContestPark.Core.Controllers
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

        private string userFullName;
        private string userId = String.Empty;

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

        public ILogger<ControllerBase> Logger { get; private set; }

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

        #endregion Properties

        #region Methods

        public override BadRequestObjectResult BadRequest(object error)
        {
            if (error is string)
            {
                return base.BadRequest(new ValidationResult(error.ToString()));
            }
            else if (error is IEnumerable<string>)
            {
                return base.BadRequest(new ValidationResult("", (IEnumerable<string>)error));
            }

            return base.BadRequest(error);
        }

        #endregion Methods
    }
}