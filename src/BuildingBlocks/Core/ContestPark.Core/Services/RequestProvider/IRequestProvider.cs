using System.Threading.Tasks;

namespace ContestPark.Core.Services.HttpService
{
    public interface IRequestProvider
    {
        Task<TResult> GetAsync<TResult>(string url);

        Task<TResult> PostAsync<TResult>(string url, object data = null);
    }
}