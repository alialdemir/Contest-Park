using ContestPark.Duel.API.Models;
using ServiceStack.Redis;
using System;
using System.Linq;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Redis.DuelUser
{
    public class DuelUserRepository : IDuelUserRepository
    {
        #region Private Variables

        private readonly IRedisClient _redisClient;

        #endregion Private Variables

        #region Constructor

        public DuelUserRepository(IRedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Aynı kategoride ve aynı bahis miktarı ve aynı bakiye tipi ile bekleyen varsa onu döndürür
        /// </summary>
        /// <param name="duelUser">Duello bilgileri</param>
        /// <returns>Bekleyen kullanıcı</returns>
        public DuelUserModel GetDuelUser(DuelUserModel duelUser)
        {
            if (duelUser == null)
                return null;

            // keyleri alt kategori id, bahis miktarı ve bakiye tipine göre filtreledik
            string key = $"Duel:SubCategoryId{duelUser.SubCategoryId.ToString()}:Bet{duelUser.Bet.ToString()}:BalanceType{duelUser.BalanceType.ToString()}*";

            // Parametreden gelen kullanıcının düellosuna karşılık gelen rakip var mı baktık
            var items = _redisClient.ScanAllKeys(key);
            if (items == null || items.ToList().Count == 0)
                return null;

            // Redis keyleri DuelUserModele çevirdik
            var duelUsers = _redisClient.GetValues<DuelUserModel>(items.ToList());

            // Kendisi ile denk gelmesin diye user id göre filtreledik
            return duelUsers.FirstOrDefault(p => p.UserId != duelUser.UserId);
        }

        /// <summary>
        /// Bekleyenlerin arasına yeni birini ekle
        /// </summary>
        /// <param name="duelUser">Bekleyecek kullanıcı bilgileri</param>
        /// <returns>Başarılı olma durumu</returns>
        public bool Insert(DuelUserModel duelUser)
        {
            bool isLock = false;

            try
            {
                string key = GetKey(duelUser);

                isLock = _redisClient.Set<DuelUserModel>(key, duelUser, expiresAt: DateTime.Now.AddSeconds(40));// 40 sn sonra redis üzerinden otomatik siler
            }
            catch (Exception ex)
            {
                isLock = true;
            }

            return isLock;
        }

        /// <summary>
        /// Redis datadan kullanıcı sil
        /// </summary>
        /// <param name="duelUser">Silinecek kullanıcı</param>
        /// <returns>Başarılı olma durumu</returns>
        public bool Delete(DuelUserModel duelUser)
        {
            if (duelUser == null)
                return false;

            string key = GetKey(duelUser);

            if (!_redisClient.ContainsKey(key))
                return false;

            return _redisClient.Remove(key);
        }

        #endregion Methods

        #region Private methds

        private string GetKey(DuelUserModel duelUserModel)
        {
            return "Duel:SubCategoryId" + duelUserModel.SubCategoryId.ToString() +
                ":Bet" + duelUserModel.Bet.ToString() +
                ":BalanceType" + duelUserModel.BalanceType.ToString() +
                ":UserId" + duelUserModel.UserId +
                ":ConnectionId" + duelUserModel.ConnectionId;
        }

        #endregion Private methds
    }
}
