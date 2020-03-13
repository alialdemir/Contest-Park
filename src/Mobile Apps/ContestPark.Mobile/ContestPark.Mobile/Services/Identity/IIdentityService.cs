using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Media;
using ContestPark.Mobile.Models.Profile;
using ContestPark.Mobile.Models.Token;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Identity
{
    public interface IIdentityService
    {
        Task ChangeCoverPictureAsync(MediaModel media);

        Task ChangeProfilePictureAsync(MediaModel media);

        Task<string> GetPhoneNumber();

        Task<ProfileInfoModel> GetProfileInfoByUserName(string userName);

        Task<RandomUserModel> GetRandomBotUser();

        Task<UserToken> GetTokenAsync(LoginModel loginModel);

        Task<string> GetUserNameByPhoneNumber(string phoneNumber);

        Task RefreshTokenAsync();

        Task<bool> SignUpAsync(SignUpModel signUpModel);

        Task Unauthorized();

        Task<bool> UpdateLanguageAsync(Enums.Languages language);

        Task<bool> UpdateUserInfoAsync(UpdateUserInfoModel userInfo);
    }
}
