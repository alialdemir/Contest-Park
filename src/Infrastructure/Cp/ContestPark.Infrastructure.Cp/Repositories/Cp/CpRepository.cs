using ContestPark.Core.Dapper;
using ContestPark.Core.Interfaces;
using ContestPark.Domain.Cp.Enums;
using ContestPark.Infrastructure.Cp.Entities;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ContestPark.Infrastructure.Cp.Repositories.Cp
{
    internal class CpRepository : DapperRepositoryBase<CpEntity>, ICpRepository
    {
        #region Private variables

        private readonly ILogger<CpRepository> _logger;

        #endregion Private variables

        #region Constructor

        public CpRepository(ISettingsBase settingsBase,
                            ILogger<CpRepository> logger) : base(settingsBase)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı Id'sine göre toplam altın miktarını verir
        /// </summary>
        /// <param name="userId">Kullanıcı Id</param>
        /// <returns>Toplam altın miktarı</returns>
        public int GetTotalGoldByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return 0;

            string sql = @"Select [Cps].[CpAmount] from [Cps] where [Cps].[UserId]=@userId";

            return Connection.Query<int>(sql, new { userId }).SingleOrDefault();
        }

        /// <summary>
        /// Kullanıcının istenilen miktarda altınını azaltır -1 dönerse sorun vardır
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="diminishingGold">Azaltmak istenilen miktar</param>
        /// <param name="goldProcessName">Hangi sebepten azaltılıyor</param>
        /// <returns>Azaldıktan sonraki altın miktarı</returns>
        public int RemoveGold(string userId, int diminishingGold, GoldProcessNames goldProcessName)
        {
            CpEntity userCp = GetCpByUserId(userId);
            if (userCp == null)
            {
                _logger.LogInformation($"Kullanıcıya ilk gold eklendi: {userId}");
                CpEntity addedCp = AddGold(userId, 0);
                return addedCp?.CpAmount ?? -1;
            }

            userCp.CpAmount -= diminishingGold;

            if (userCp.CpAmount < 0)
            {
                _logger.LogWarning($"Bahis çıkarılınca altın miktarı negatif oldu.. userId: {userId}");
                userCp.CpAmount = 0;
            }

            Update(userCp);

            return userCp.CpAmount;
        }

        /// <summary>
        /// User id göre Cp entity verir
        /// </summary>
        private CpEntity GetCpByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning($"Kullanıcı id boş geldi. UserId: {userId}");
                return null;
            }

            string sql = "select * from [Cps] where [Cps].[UserId]=@userId";
            CpEntity userCp = Connection.QueryFirstOrDefault<CpEntity>(sql, new { userId });

            return userCp;
        }

        /// <summary>
        /// Kullanıcıya altın ekle
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gold"></param>
        /// <returns></returns>
        private CpEntity AddGold(string userId, int gold)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning($"Kullanıcı id boş geldi. UserId: {userId}");

                return null;
            }

            if (gold < 0)
            {
                _logger.LogWarning($"Sıfırdan az miktarda altın eklemeye çalışıldı altın miktarı sıfırlandı. userId: {userId}");
                gold = 0;
            }

            CpEntity cp = new CpEntity
            {
                UserId = userId,
                CpAmount = gold,
            };

            string newUserId = Insert(cp);

            if (string.IsNullOrEmpty(newUserId))
            {
                _logger.LogWarning($"Altın ekleme işlemi başarısız oldu. UserId: {userId}");

                return null;
            }

            return cp;
        }

        #endregion Methods
    }
}