using ContestPark.Mobile.Models;
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

        Task<bool> ChangePasswordAsync(ChangePasswordModel changePasswordModel);

        Task<bool> ChangePasswordAsync(int code);

        Task ChangeProfilePictureAsync(MediaModel media);

        Task ForgetYourPasswordAsync(string userNameOrEmailAddress);

        Task<ProfileInfoModel> GetProfileInfoByUserName(string userName);

        Task<UserToken> GetTokenAsync(LoginModel loginModel);

        Task RefreshTokenAsync();

        Task<bool> SignUpAsync(SignUpModel signUpModel);

        Task Unauthorized();

        Task<bool> UpdateUserInfoAsync(UpdateUserInfoModel userInfo);
    }
}
