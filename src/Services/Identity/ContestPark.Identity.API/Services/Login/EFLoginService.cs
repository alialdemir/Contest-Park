using ContestPark.Identity.API.Data.Tables;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services.Login
{
    public class EFLoginService : ILoginService<ApplicationUser>
    {
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;

        public EFLoginService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ApplicationUser> FindByUsername(string user)
        {
            return await _userManager.FindByEmailAsync(user);
        }

        public Task SignIn(ApplicationUser user)
        {
            return _signInManager.SignInAsync(user, true);
        }

        public async Task<bool> ValidateCredentials(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}