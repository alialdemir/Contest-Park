using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Token;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Identity
{
    public interface IIdentityService
    {
        Task<UserToken> GetTokenAsync(LoginModel loginModel);
    }
}