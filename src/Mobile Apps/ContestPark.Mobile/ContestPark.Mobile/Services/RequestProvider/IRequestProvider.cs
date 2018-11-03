using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.RequestProvider
{
    public interface IRequestProvider
    {
        Task<TResult> DeleteAsync<TResult>(string url, object data = null);

        Task<TResult> GetAsync<TResult>(string url);

        Task<TResult> PostAsync<TResult>(string url, object data = null);

        Task<TResult> PostAsync<TResult>(string url, Dictionary<string, string> dictionary);

        Task<TResult> PostAsync<TResult>(string url, Stream file);
    }
}