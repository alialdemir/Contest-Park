using ContestPark.Balance.API.Infrastructure.Documents;
using ContestPark.Balance.API.Models;
using ContestPark.Core.CosmosDb.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Infrastructure.Repositories.Balance
{
    public class BalanceRepository : IBalanceRepository
    {
        #region Private Variables

        private readonly IDocumentDbRepository<Documents.Balance> _balanceRepository;
        private readonly IDocumentDbRepository<BalanceHistory> _balanceHistoryRepository;
        private readonly ILogger<BalanceRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public BalanceRepository(IDocumentDbRepository<Documents.Balance> balanceRepository,
                                 IDocumentDbRepository<BalanceHistory> balanceHistoryRepository,
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

        public IEnumerable<BalanceModel> GetUserBalances(string userId)
        {
            string sql = @"SELECT ba.Amount, ba.BalanceType
                           FROM c JOIN ba IN c.BalanceAmounts
                           WHERE c.UserId=@userId";

            return _balanceRepository.QueryMultiple<BalanceModel>(sql, new
            {
                userId
            });
        }

        /// <summary>
        /// Kullanıcı id'ye ait bakiye bilgisini amount tutarı kadar ekler/çıkarır
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="amount">Bakiye tutarı</param>
        public async Task<bool> UpdateBalanceAsync(ChangeBalanceModel changeBalance)
        {
            _logger.LogInformation("Bakiye yükleme işlemi başlatılıyor...",
                                   changeBalance.UserId,
                                   changeBalance.Amount,
                                   changeBalance.BalanceType,
                                   changeBalance.BalanceHistoryType);

            string sql = "SELECT * FROM c WHERE c.UserId=@userId";
            Documents.Balance balance = _balanceRepository.QuerySingle<Documents.Balance>(sql, new
            {
                userId = changeBalance.UserId
            });
            if (balance == null)// Eğer ilk defa bakiye işlemi yapılacaksa null gelir o zaman yeni instance oluşturur
            {
                balance = new Documents.Balance
                {
                    Id = "-1",
                    UserId = changeBalance.UserId,
                };
            }

            var balanceAmount = balance
                                    .BalanceAmounts
                                    .FirstOrDefault(x => x.BalanceType == changeBalance.BalanceType);
            if (balanceAmount == null)
            {
                balanceAmount = new BalanceAmount
                {
                    BalanceType = changeBalance.BalanceType,
                    Amount = 0
                };
            }

            decimal oldAmount = balanceAmount.Amount;

            /*
             * Eğer bahis düşmet isterse amount negatif değer gelir o zamanda yine çıkarma olur
             * pozitif ise toplar  bu sayede tek methodan altın ekle çıkar yapabildik
             */
            balanceAmount.Amount += changeBalance.Amount;

            bool isSuccess = await AddOrUpdateBalance(balance, oldAmount, balanceAmount.Amount);
            if (isSuccess)
            {
                isSuccess = await AddBalanceHistory(changeBalance, oldAmount, balanceAmount.Amount);
            }

            _logger.LogInformation($"Bakiye yükleme işlemi bitti. Status: {isSuccess}",
                                   changeBalance.UserId,
                                   changeBalance.Amount,
                                   changeBalance.BalanceType,
                                   changeBalance.BalanceHistoryType);

            return isSuccess;
        }

        /// <summary>
        /// Bakiye geçmişine ekleme yapar
        /// </summary>
        /// <summary>
        /// Eğer id -1 ise ekleme yapar değilse güncelleme işlemi yapar
        /// </summary>
        /// <param name="balance">Kullanıcı bakiye bilgileri</param>
        /// <param name="oldAmount">Eski bakiye</param>
        /// <param name="newAmount">Yeni bakiye</param>
        /// <returns>Başarılı ise true değilse false</returns>
        private async Task<bool> AddBalanceHistory(ChangeBalanceModel balance, decimal oldAmount, decimal newAmount)
        {
            bool isSuccess = await _balanceHistoryRepository.AddAsync(new BalanceHistory
            {
                BalanceType = balance.BalanceType,
                BalanceHistoryType = balance.BalanceHistoryType,
                UserId = balance.UserId,
                OldAmount = oldAmount,
                NewAmount = newAmount
            });

            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: Kullanıcı bakiyesi history eklenirken hata oluştu.",
                                    balance.UserId,
                                    oldAmount,
                                    newAmount,
                                    balance.BalanceType,
                                    balance.BalanceHistoryType);
            }

            return isSuccess;
        }

        /// <summary>
        /// Eğer id -1 ise ekleme yapar değilse güncelleme işlemi yapar
        /// </summary>
        /// <param name="balance">Kullanıcı bakiye bilgileri</param>
        /// <param name="oldAmount">Eski bakiye</param>
        /// <param name="newAmount">Yeni bakiye</param>
        /// <returns>Başarılı ise true değilse false</returns>
        private async Task<bool> AddOrUpdateBalance(Documents.Balance balance, decimal oldAmount, decimal newAmount)
        {
            bool isSuccess = false;

            if (balance.Id == "-1")// ChangeBalanceByUserId içinde yeni instance oluşturduğumda id -1 verdim  eğer -1 ise yeni kayıttır
            {
                balance.Id = Guid.NewGuid().ToString();
                isSuccess = await _balanceRepository.AddAsync(balance);
            }
            else
            {
                isSuccess = await _balanceRepository.UpdateAsync(balance);
            }

            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: Kullanıcı bakiyesi güncelleme/ekleme sırasında hata oluştu.",
                                    balance.UserId,
                                    oldAmount,
                                    newAmount);
            }

            return isSuccess;
        }

        #endregion Methods
    }
}