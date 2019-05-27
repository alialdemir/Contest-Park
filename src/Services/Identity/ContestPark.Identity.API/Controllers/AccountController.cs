using ContestPark.Core.Enums;
using ContestPark.Identity.API.Data.Repositories.User;
using ContestPark.Identity.API.Models;
using ContestPark.Identity.API.Resources;
using ContestPark.Identity.API.Services;
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
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : Core.Controllers.ControllerBase
    {
        #region Private variables

        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;

        #endregion Private variables

        #region Constructor

        public AccountController(IEmailService emailService,
                                 ILogger<AccountController> logger,
                                 UserManager<ApplicationUser> userManager,
                                 IUserRepository userRepository) : base(logger)
        {
            _emailService = emailService;
            _logger = logger;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Şifremi unuttum
        /// </summary>
        /// <param name="model">Kullanıcı adı veya eposta adresi</param>
        /// <returns>
        /// Güncelleme başarılı olduysa başarılı yanıtı döndürür,
        /// aksi takdirde hatanın hata nedenlerini döndürür
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotYourPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgetPasswordAsync([FromBody]ForgetPasswordModel model)
        {
            if (string.IsNullOrEmpty(model.UserNameOrEmail))
                return BadRequest();

            ApplicationUser user = null;
            if (new EmailAddressAttribute().IsValid(model.UserNameOrEmail))
            {
                user = await _userManager.FindByEmailAsync(model.UserNameOrEmail.ToLower());
            }
            else
            {
                user = await _userManager.FindByNameAsync(model.UserNameOrEmail.ToLower());
            }

            if (user == null)
                return BadRequest(IdentityResource.UserNotFound);

            int code = _userRepository.InsertCode(user.Id);
            if (code == 0)
            {
                _logger.LogError("Şifre değiştirme sırasında code üretilemedi.");
                return BadRequest(IdentityResource.ThereWasAnErrorInSendingPassword);
            }

            string message = ForgotPasswordEmailMessageHtml(user.FullName, code);
            bool isSendMail = await _emailService.SendEmailAsync(user.Email, "Forgot your password?", message);
            if (!isSendMail)
            {
                _logger.LogError("Şifre değiştir maili gönderilemedi.");
                return BadRequest(IdentityResource.ThereWasAnErrorInSendingPassword);
            }

            return Ok();
        }

        /// <summary>
        /// Üye ol
        /// </summary>
        /// <param name="signUpModel">Üye olunacak model</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUp([FromForm]SignUpModel signUpModel)
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

        /// <summary>
        /// Şifremi unuttum email içeriği
        /// </summary>
        /// <param name="fullname"></param>
        /// <param name="code"></param>
        /// <returns>Html mesaj</returns>
        private string ForgotPasswordEmailMessageHtml(string fullname, int code)
        {
            return $@"
                    {IdentityResource.Hello} {fullname},<br/><br/>
                    {IdentityResource.WeHaveReceivedARequestToRenewYourContestParkPassword}</br>
                    {IdentityResource.EnterTheCodeBelowToRefreshThePassword}<br/><br/>

                    <div style='code-box'>
                          {code}
                    </div><br/><br/>
                   ";
        }

        #endregion Methods
    }
}