using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Profile;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Services.RequestProvider;
using ContestPark.Mobile.Services.Settings;
using System.IO;
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

        public Task ChangeCoverPictureAsync(Stream picture)
        {
            return Task.CompletedTask;
        }

        public Task<bool> ChangePasswordAsync(ChangePasswordModel changePasswordModel)
        {
            return Task.FromResult(true);
        }

        public Task ChangeProfilePictureAsync(Stream picture)
        {
            return Task.CompletedTask;
        }

        public Task ForgetYourPasswordAsync(string userNameOrEmailAddress)
        {
            return Task.CompletedTask;
        }

        public Task<ProfileInfoModel> GetProfileInfoByUserName(string userId)
        {
            return Task.FromResult(new ProfileInfoModel
            {
                CoverPicture = DefaultImages.DefaultCoverPicture,
                ProfilePicturePath = DefaultImages.DefaultProfilePicture,
                FollowersCount = 99,
                FollowUpCount = 101,
                FullName = "Ali Aldemir",
                GameCount = 9876,
                IsFollowing = false,
                UserId = userId
            });
        }

        public async Task<UserToken> GetTokenAsync(LoginModel loginModel)
        {
            await Task.Delay(3590);

            if (!(loginModel.UserName.ToLower() == "witcherfearless" && loginModel.Password == "19931993"))
                throw new HttpRequestExceptionEx(System.Net.HttpStatusCode.BadRequest, "invalid_username_or_password");

            return new UserToken
            {
                AccessToken = "fake_token",
                ExpiresIn = 365,
                IdToken = "123",
                RefreshToken = "fake_refresh_token",
                TokenType = "bearer"
            };
        }

        public Task RefreshTokenAsync()
        {
            return Task.CompletedTask;
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

        public Task<bool> UpdateUserInfoAsync(UpdateUserInfoModel userInfo)
        {
            return Task.FromResult(true);
        }
    }
}