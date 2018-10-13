using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Cache
{
    public interface ICacheService
    {
        void Add<T>(string key, T data);

        bool IsExpired(string key);

        Task<T> Get<T>(string key);

        void Empty(string key);
    }
}