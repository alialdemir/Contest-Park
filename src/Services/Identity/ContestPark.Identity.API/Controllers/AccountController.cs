using ContestPark.Core.Enums;
using ContestPark.Identity.API.Models;
using ContestPark.Identity.API.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Core.Controllers.ControllerBase
    {
        //private readonly InMemoryUserLoginService _loginService;
        private readonly ILogger<AccountController> _logger;

        private readonly UserManager<ApplicationUser> _userManager;

        #region Constructor

        public AccountController(
         ILogger<AccountController> logger,
         UserManager<ApplicationUser> userManager) : base(logger)
        {
            _logger = logger;
            _userManager = userManager;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Üye ol
        /// </summary>
        /// <param name="signUpModel">Üye olunacak model</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromForm]SignUpModel signUpModel)
        {
            if (signUpModel == null)
                return BadRequest();

            ApplicationUser user = new ApplicationUser
            {
                UserName = signUpModel.UserName.ToLower(),
                FullName = signUpModel.FullName.ToLowerInvariant(),
                Email = signUpModel.Email.ToLower(),
                LanguageCode = signUpModel.LanguageCode.ToLower(),
            };

            var result = await _userManager.CreateAsync(user, signUpModel.Password);
            if (result.Errors.Count() > 0)
            {
                var errors = result.Errors.Select(e =>
                        CurrentUserLanguage.HasFlag(Languages.Turkish) ? IdentityResource.ResourceManager.GetString(e.Code) : e.Description
                        ).ToList();

                return BadRequest(errors);
            }

            _logger.LogInformation($"Register new user: {user.Id} userName: {user.UserName}");

            return Ok();
        }

        #endregion Methods
    }
}