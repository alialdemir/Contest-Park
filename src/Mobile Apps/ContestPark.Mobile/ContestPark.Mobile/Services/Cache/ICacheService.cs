using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Cache
{
    public interface ICacheService
    {
        bool IsExpired(string key);

        T Get<T>(string key);

        void Empty(string key);

        void EmptyAll();

        void EmptyByKey(string key);

        void Add<T>(string key, T data, TimeSpan? expireIn = null);
    }
}
