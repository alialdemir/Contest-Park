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
        ///  Eski şifre ile şifre değiştir
        /// </summary>
        /// <param name="changePasswordModel">Şuanki şifre ve yeni şifre</param>
        /// <returns>
        /// Güncelleme başarılı olduysa başarılı yanıtı döndürür,
        /// aksi takdirde hatanın hata nedenlerini döndürür
        /// </returns>
        [HttpPost]
        [Route("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel changePasswordModel)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return NotFound();

            bool isPasswordSuccess = await _userManager.CheckPasswordAsync(user, changePasswordModel.OldPassword);

            if (!isPasswordSuccess)
                return BadRequest(IdentityResource.YourCurrentPasswordIsInvalid);

            IdentityResult result = await ResetPasswordAsync(user, changePasswordModel.NewPassword);
            if (!result.Succeeded)
                return BadRequest(IdentityResource.ErrorChangingPasswordPleaseTryAgain);

            return Ok();
        }

        /// <summary>
        /// Şifremi unuttum code kontrol ise şifre değiştir
        /// </summary>
        /// <param name="changePasswordModel">Kullanıcı adı veya eposta adresi</param>
        /// <returns>
        /// Güncelleme başarılı olduysa başarılı yanıtı döndürür,
        /// aksi takdirde hatanın hata nedenlerini döndürür
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("ChangePassword/code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePasswordWithCode([FromBody]ChangePasswordWithCodeModel changePasswordModel)
        {
            ApplicationUser user = _userRepository.GetUserByCode(changePasswordModel.Code);
            if (user == null)
                return NotFound();

            IdentityResult result = await ResetPasswordAsync(user, changePasswordModel.Password);
            if (!result.Succeeded)
                return BadRequest(IdentityResource.ErrorChangingPasswordPleaseTryAgain);

            _userRepository.RemoveCode(user.Id);

            return Ok();
        }

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
        /// Şifremi unuttum code kontrol
        /// </summary>
        /// <param name="model">Kullanıcı adı veya eposta adresi</param>
        /// <returns>
        /// Güncelleme başarılı olduysa başarılı yanıtı döndürür,
           /// aksi takdirde hatanın hata nedenlerini döndürür
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("ForgotYourPassword/codecheck")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ForgetPasswordCodeCheckAsync([FromQuery]int code)
        {
            bool isSuccess = _userRepository.CodeCheck(code);
            if (isSuccess)
                return Ok();

            return NotFound();
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

        /// <summary>
        /// Kullanıcının şifresini değiştir
        /// </summary>
        /// <param name="user">Şifresi değişecek kullanıcı</param>
        /// <param name="newPassword">Yeni şifre</param>
        /// <returns>İşem sonucu</returns>
        private async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            string code = await _userManager.GeneratePasswordResetTokenAsync(user);// şifre değiştirme için kod oluşturuldu
            IdentityResult result = await _userManager.ResetPasswordAsync(user, code, newPassword);// şifre değiştirildi edildi

            return result;
        }

        #endregion Methods
    }
}