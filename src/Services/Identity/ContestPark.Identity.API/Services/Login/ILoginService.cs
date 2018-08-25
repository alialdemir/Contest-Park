using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services.Login
{
    public interface ILoginService<T>
    {
        Task<bool> ValidateCredentials(T user, string password);

        Task<T> FindByUsername(string user);

        Task SignIn(T user);
    }
}