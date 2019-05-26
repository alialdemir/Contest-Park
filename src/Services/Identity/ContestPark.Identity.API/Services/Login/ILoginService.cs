using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services.Login
{
    public interface ILoginService<T>
    {
        Task<T> FindByUsername(string user);

        Task SignIn(T user);

        Task<bool> ValidateCredentials(T user, string password);
    }
}