using Newtonsoft.Json;
using ServiceStack.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Signalr.Repositories.DuelUser
{
    public class DuelUserRepository : IDuelUserRepository
    {
        #region Private Variables

        private readonly string _key = "DuelUser";

        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        #endregion Private Variables

        #region Constructor

        public DuelUserRepository(ConnectionMultiplexer redis)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _database = redis.GetDatabase();
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Beklemede olan tüm kullanıcıları getirir
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entities.DuelUser>> GetAllAsync()
        {
            string duelUsers = await _database.StringGetAsync(_key);

            if (String.IsNullOrEmpty(duelUsers))
                return new List<Entities.DuelUser>();

            return JsonConvert.DeserializeObject<List<Entities.DuelUser>>(duelUsers);
        }

        /// <summary>
        /// Aynı kategoride ve aynı bahis miktarı ile bekleyen varsa onu döndürür
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId"> Alt kategori id</param>
        /// <param name="bet">Bahis tutarı</param>
        /// <returns>Bekleyen kullanıcı</returns>
        public async Task<Entities.DuelUser> GetDuelUserAsync(string userId, Int16 subCategoryId, int bet)
        {
            List<Entities.DuelUser> duelUsers = await GetAllAsync();
            IRedisClientsManager d =
            return duelUsers.FirstOrDefault(p => p.UserId != userId && p.SubCategoryId == subCategoryId && p.Bet == bet);
        }

        /// <summary>
        /// Bekleyenlerin arasına yeni birini ekle
        /// </summary>
        /// <param name="duelUserModel">Bekleyecek kullanıcı biilgileri</param>
        /// <returns>Başarılı olma durumu</returns>
        public async Task<bool> InsertAsync(Entities.DuelUser duelUser)
        {
            List<Entities.DuelUser> duelUsers = await GetAllAsync();

            if (duelUsers.Any(x => x.ConnectionId == duelUser.ConnectionId))// todo connection id değişirse aynı kullanıcı iki kere eklenebilir
            {
                duelUsers.Remove(duelUsers.FirstOrDefault(p => p.ConnectionId == duelUser.ConnectionId));
            }

            duelUsers.Add(duelUser);

            return await SetStringDuelUserListAsync(duelUsers);
        }

        /// <summary>
        /// Redis datadan kullanıcı sil
        /// </summary>
        /// <param name="duelUserModel">Silinecek kullanıcı</param>
        /// <returns>Başarılı olma durumu</returns>
        public async Task<bool> DeleteAsync(Entities.DuelUser duelUser)
        {
            List<Entities.DuelUser> duelUsers = await GetAllAsync();

            if (duelUsers == null || duelUsers.Count <= 0)
                return false;

            duelUsers.Remove(duelUser);

            return await SetStringDuelUserListAsync(duelUsers);
        }

        /// <summary>
        /// Bekleme süresi 30 saniyeden büyük olan kullanıcıları siler
        /// </summary>
        public async void ClearExpiredDuelUserList()
        {
            var duelUsers = await GetAllAsync();
            DateTime now = DateTime.Now;

            duelUsers = duelUsers.Where(p => (now - p.Date).TotalSeconds < 30).ToList();

            await SetStringDuelUserListAsync(duelUsers);
        }

        #endregion Methods

        #region Private methds

        private Task<bool> SetStringDuelUserListAsync(List<Entities.DuelUser> duelUsers)
        {
            string serialize = JsonConvert.SerializeObject(duelUsers);

            return _database.StringSetAsync(_key, serialize);
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }

        #endregion Private methds
    }
}