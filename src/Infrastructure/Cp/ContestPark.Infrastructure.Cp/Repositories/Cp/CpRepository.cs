using ContestPark.Core.Dapper;
using ContestPark.Core.Interfaces;
using ContestPark.Domain.Cp.Enums;
using ContestPark.Infrastructure.Cp.Entities;
using ContestPark.Infrastructure.Cp.Repositories.CpInfo;
using Dapper;
using Microsoft.Extensions.Logging;
using System;

namespace ContestPark.Infrastructure.Cp.Repositories.Cp
{
    internal class CpRepository : DapperRepositoryBase<CpEntity>, ICpRepository
    {
        #region Private variables

        private readonly ILogger<CpRepository> _logger;

        private readonly ICpInfoRepository _cpInfoRepository;

        #endregion Private variables

        #region Constructor

        public CpRepository(ISettingsBase settingsBase,
                            ICpInfoRepository cpInfoRepository,
                            ILogger<CpRepository> logger) : base(settingsBase)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cpInfoRepository = cpInfoRepository;
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

            string sql = @"Select top 1 [Cps].[CpAmount] from [Cps] where [Cps].[UserId]=@userId";

            return Connection.QuerySingleOrDefault<int>(sql, new { userId });
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

            bool isSuccess = Update(userCp);

            if (isSuccess)
            {
                _cpInfoRepository.Insert(new CpInfoEntity
                {
                    CpSpent = -diminishingGold,
                    ChipProcessName = goldProcessName,
                    UserId = userId,
                });
            }

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

            string sql = "select top 1 * from [Cps] where [Cps].[UserId]=@userId";
            CpEntity userCp = Connection.QuerySingleOrDefault<CpEntity>(sql, new { userId });

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

            //if (gold < 0)
            //{
            //    _logger.LogWarning($"Sıfırdan az miktarda altın eklemeye çalışıldı altın miktarı sıfırlandı. userId: {userId}");
            //    gold = 0;
            //}

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

        /// <summary>
        /// Kullanıcıya altın ekler
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="addedChips">Eklenecek altın miktarı</param>
        /// <param name="goldProcessName">Hangi sebepten artıyor</param>
        /// <returns>Azaldıktan sonraki altın miktarı</returns>
        public int AddGold(string userId, int addedChips, GoldProcessNames goldProcessName)
        {
            CpEntity userCp = GetCpByUserId(userId);
            if (userCp == null)
            {
                _logger.LogInformation($"Kullanıcıya ilk gold eklendi: {userId}");
                CpEntity addedCp = AddGold(userId, 0);
                return addedCp?.CpAmount ?? -1;
            }

            userCp.CpAmount += addedChips;

            bool isSuccess = Update(userCp);
            if (isSuccess)
            {
                _cpInfoRepository.Insert(new CpInfoEntity
                {
                    CpSpent = addedChips,
                    ChipProcessName = goldProcessName,
                    UserId = userId,
                });
            }

            return userCp.CpAmount;
        }

        #endregion Methods
    }
}