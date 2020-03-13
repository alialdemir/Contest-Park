using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Media;
using ContestPark.Mobile.Models.Profile;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Services.Settings;
using Prism.Services;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Identity
{
    public class IdentityMockService : IIdentityService
    {
        #region Private varaibles

        private readonly ISettingsService _settingsService;
        private readonly IPageDialogService _dialogService;

        #endregion Private varaibles

        #region Constructor

        public IdentityMockService(ISettingsService settingsService,
            IPageDialogService dialogService
            )
        {
            _settingsService = settingsService;
            _dialogService = dialogService;
        }

        #endregion Constructor

        public Task ChangeCoverPictureAsync(MediaModel picture)
        {
            return Task.CompletedTask;
        }

        public Task ChangeProfilePictureAsync(MediaModel picture)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumber()
        {
            return Task.FromResult("5444261154");
        }

        public Task<ProfileInfoModel> GetProfileInfoByUserName(string userId)
        {
            return Task.FromResult(new ProfileInfoModel
            {
                CoverPicture = DefaultImages.DefaultCoverPicture,
                ProfilePicturePath = DefaultImages.DefaultProfilePicture,
                FollowersCount = "99",
                FollowUpCount = "101",
                FullName = "Ali Aldemir",
                GameCount = "9876",
                IsFollowing = false,
                UserId = userId,
                IsBlocked = true,
            });
        }

        public Task<RandomUserModel> GetRandomBotUser()
        {
            return Task.FromResult(new RandomUserModel
            {
                FullName = "Elif Öz",
                ProfilePicturePath = DefaultImages.DefaultProfilePicture,
                UserId = "2222-2222-2222-2222",
            });
        }

        public async Task<UserToken> GetTokenAsync(LoginModel loginModel)
        {
            await Task.Delay(3590);

            return new UserToken
            {
                AccessToken = "fake_token",
                ExpiresIn = 365,
                RefreshToken = "fake_refresh_token",
                TokenType = "bearer"
            };
        }

        public Task<string> GetUserNameByPhoneNumber(string phoneNumber)
        {
            return Task.FromResult("");
            //return Task.FromResult("witcherfearless");
        }

        public Task RefreshTokenAsync()
        {
            return Task.CompletedTask;
        }

        public async Task<bool> SignUpAsync(SignUpModel signUpModel)
        {
            if (_settingsService.SignUpCount > 3)// Sürekli üye olup davetiye kodu ile para kasmasınlar diye bir cihazdan 3 kere üye olma hakkı verdik :)
            {
                await _dialogService.DisplayAlertAsync("", ContestParkResources.GlobalErrorMessage, ContestParkResources.Okay);

                _settingsService.SignUpCount = 0;

                return false;
            }

            return true;
        }

        public Task Unauthorized()
        {
            _settingsService.AuthAccessToken = string.Empty;
            _settingsService.SignalRConnectionId = string.Empty;

            _settingsService.RemoveCurrentUser();

            return Task.CompletedTask;
        }

        public Task<bool> UpdateLanguageAsync(Languages language)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UpdateUserInfoAsync(UpdateUserInfoModel userInfo)
        {
            return Task.FromResult(true);
        }
    }
}
