﻿using ContestPark.Balance.API.Infrastructure.Tables;
using ContestPark.Balance.API.Models;
using ContestPark.Core.Database.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Infrastructure.Repositories.Balance
{
    public class BalanceRepository : IBalanceRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Balance> _balanceRepository;
        private readonly IRepository<BalanceHistory> _balanceHistoryRepository;
        private readonly ILogger<BalanceRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public BalanceRepository(IRepository<Tables.Balance> balanceRepository,
                                 IRepository<BalanceHistory> balanceHistoryRepository,
                                 ILogger<BalanceRepository> logger)
        {
            _balanceRepository = balanceRepository;
            _balanceHistoryRepository = balanceHistoryRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcının tüm bakiye bilgilerini döner
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Bakiye bilgileri</returns>

        public BalanceModel GetUserBalances(string userId)
        {
            return _balanceRepository.SpQuerySingleOrDefault<BalanceModel>("SP_GetBalance", new
            {
                userId
            });
        }

        /// <summary>
        /// Kullanıcı id'ye ait bakiye bilgisini amount tutarı kadar ekler/çıkarır
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="amount">Bakiye tutarı</param>
        public Task<bool> UpdateBalanceAsync(ChangeBalanceModel changeBalance)
        {
            _logger.LogInformation("Bakiye yükleme işlemi başlatılıyor...",
                                   changeBalance.UserId,
                                   changeBalance.Amount,
                                   changeBalance.BalanceType,
                                   changeBalance.BalanceHistoryType);

            return _balanceRepository.ExecuteAsync("SP_UpdateBalance", new
            {
                changeBalance.Amount,
                BalanceType = (byte)changeBalance.BalanceType,
                BalanceHistoryType = (byte)changeBalance.BalanceHistoryType,
                changeBalance.UserId,
            });
        }

        #endregion Methods
    }
}
