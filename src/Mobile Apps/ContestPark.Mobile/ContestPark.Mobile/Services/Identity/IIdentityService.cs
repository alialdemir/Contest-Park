using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Token;
using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Identity
{
    public interface IIdentityService
    {
        Task ChangeCoverPictureAsync(Stream picture);

        Task<bool> ChangePasswordAsync(ChangePasswordModel changePasswordModel);

        Task ChangeProfilePictureAsync(Stream picture);

        Task ForgetYourPasswordAsync(string userNameOrEmailAddress);

        Task<UserToken> GetTokenAsync(LoginModel loginModel);

        Task RefreshTokenAsync();

        Task<bool> SignUpAsync(SignUpModel signUpModel);

        Task Unauthorized();

        Task<bool> UpdateUserInfoAsync(UpdateUserInfoModel userInfo);
    }
}