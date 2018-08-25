using ContestPark.Mobile.Models;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.RequestProvider
{
    public interface IRequestProvider
    {
        Task<TResult> GetAsync<TResult>(string url);

        Task<TResult> PostAsync<TResult>(string url, object data = null);

        Task<TResult> DeleteAsync<TResult>(string url, object data = null);

        Task<TResult> PostAsync<TResult>(string url, string clientId, string scopes, LoginModel loginModel);
    }
}