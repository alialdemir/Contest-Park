using ContestPark.Mobile.Configs;
using MonkeyCache.SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Cache
{
    public class CacheService : ICacheService
    {
        public CacheService()
        {
            Barrel.ApplicationId = "ContestPark";
        }

        /// <summary>
        /// Adds an entry to the barrel default 30 minutes on expires time
        /// </summary>
        /// <typeparam name="T">Added type</typeparam>
        /// <param name="key">Unique identifier for the entry</param>
        /// <param name="data">Data object to store</param>
        /// <param name="expireIn">Time from UtcNow to expire entry in</param>
        public void Add<T>(string key, T data)
        {
            if (data != null)
            {
                Barrel.Current.Add(key, data, GlobalSetting.Instance.CacheExpireIn);
            }
        }

        /// <summary>
        /// param list of keys to flush
        /// </summary>
        /// <param name="key">Unique identifier for the entry</param>
        public void Empty(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                Barrel.Current.Empty(key);
            }
        }

        /// <summary>
        /// param list of keys to flush
        /// </summary>
        public void EmptyAll()
        {
            Barrel.Current.EmptyAll();
        }

        /// <summary>
        /// Gets the data entry for the specified key.
        /// </summary>
        /// <param name="key">Unique identifier for the entry to get</param>
        /// <returns>The data object that was stored if found, else default(T)</returns>
        public Task<T> Get<T>(string key)
        {
            return Task.Run<T>(() =>
            {
                return Barrel.Current.Get<T>(key: key);
            });
        }

        /// <summary>
        /// Checks to see if the entry for the key is expired.
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>If the expiration data has been met</returns>
        public bool IsExpired(string key)
        {
            return Barrel.Current.IsExpired(key);
        }

        /// <summary>
        /// Key değerini içeren cache'leri siler
        /// </summary>
        /// <param name="key">Key</param>
        public void EmptyByKey(string key)
        {
            IEnumerable<string> keys = Barrel.Current.GetKeys();
            if (keys == null || !keys.Any(x => x.Contains(key)))
                return;

            Barrel.Current.Empty(keys.Where(x => x.Contains(key)).ToArray());
        }
    }
}
