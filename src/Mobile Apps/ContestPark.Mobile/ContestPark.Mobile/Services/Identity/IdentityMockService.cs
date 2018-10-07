using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Services.Settings;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Identity
{
    public class IdentityMockService : IIdentityService
    {
        #region Private varaibles

        private readonly ISettingsService _settingsService;

        #endregion Private varaibles

        #region Constructor

        public IdentityMockService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        #endregion Constructor

        public Task ForgetYourPasswordAsync(string userNameOrEmailAddress)
        {
            return Task.CompletedTask;
        }

        public Task<UserToken> GetTokenAsync(LoginModel loginModel)
        {
            return Task.FromResult(new UserToken
            {
                AccessToken = "fake_token",
                ExpiresIn = 365,
                IdToken = "123",
                RefreshToken = "fake_refresh_token",
                TokenType = "bearer"
            });
        }

        public Task<bool> SignUpAsync(SignUpModel signUpModel)
        {
            return Task.FromResult(true);
        }

        public Task Unauthorized()
        {
            _settingsService.AuthAccessToken = string.Empty;
            _settingsService.SignalRConnectionId = string.Empty;

            _settingsService.RemoveCurrentUser();

            return Task.CompletedTask;
        }
    }
}