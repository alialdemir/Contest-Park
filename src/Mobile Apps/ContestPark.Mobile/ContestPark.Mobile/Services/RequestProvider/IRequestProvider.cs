using ContestPark.Mobile.Models.RequestProvider;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.RequestProvider
{
    public interface IRequestProvider
    {
        Task<ResponseModel<TResult>> DeleteAsync<TResult>(string url, object data = null);

        Task<ResponseModel<TResult>> GetAsync<TResult>(string url);

        Task<ResponseModel<TResult>> PostAsync<TResult>(string url, object data = null);

        Task<ResponseModel<TResult>> PostAsync<TResult>(string url, Dictionary<string, string> dictionary);

        Task<ResponseModel<TResult>> PostAsync<TResult>(string url, Stream file);
    }
}
