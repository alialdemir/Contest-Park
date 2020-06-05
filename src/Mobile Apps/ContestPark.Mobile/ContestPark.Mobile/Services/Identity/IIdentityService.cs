using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Media;
using ContestPark.Mobile.Models.Profile;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Models.User;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Identity
{
    public interface IIdentityService
    {
        Task ChangeCoverPictureAsync(MediaModel media);

        Task ChangeProfilePictureAsync(MediaModel media);

        Task<string> GetPhoneNumber();

        Task<ProfileInfoModel> GetProfileInfoByUserName(string userName, bool isForceCache = false);

        Task<RandomUserModel> GetRandomBotUser();

        Task<UserToken> GetTokenAsync(LoginModel loginModel);

        Task<UserInfoModel> GetUserInfo();

        Task RefreshTokenAsync();

        Task<bool> SignUpAsync(SignUpModel signUpModel);

        Task Unauthorized();

        Task<bool> UpdateLanguageAsync(Enums.Languages language);

        Task<bool> UpdateUserInfoAsync(UpdateUserInfoModel userInfo);
    }
}
