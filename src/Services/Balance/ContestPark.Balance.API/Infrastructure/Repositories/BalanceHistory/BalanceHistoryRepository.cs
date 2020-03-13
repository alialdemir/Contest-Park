using ContestPark.Balance.API.Enums;
using ContestPark.Core.Database.Interfaces;

namespace ContestPark.Balance.API.Infrastructure.Repositories.BalanceHistory
{
    public class BalanceHistoryRepository : IBalanceHistoryRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.BalanceHistory> _balanceHistoryRepository;

        #endregion Private Variables

        #region Constructor

        public BalanceHistoryRepository(IRepository<Tables.BalanceHistory> balanceHistoryRepository)
        {
            _balanceHistoryRepository = balanceHistoryRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Günlük altın alma hakkı açılmış mı kontrol eder
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Eğer 12 saatlik altın alma hakkı açılmışsa true açılmamışsa false döner</returns>
        public bool IsReward(string userId)
        {
            string sql = @"SELECT ISNULL((
                           SELECT case when
                           HOUR(TIMEDIFF(NOW(), bh.CreatedDate))  >= 12
                           then NULL
                           ELSE 1
                           END
                           FROM BalanceHistories bh
                           WHERE (bh.BalanceHistoryType = @balanceHistoryType
                           AND bh.UserId = @userId)
                           ORDER BY bh.CreatedDate DESC
                           LIMIT 0,1))";

            return _balanceHistoryRepository.QuerySingleOrDefault<bool>(sql, new
            {
                balanceHistoryType = (byte)BalanceHistoryTypes.DailyChip,
                userId,
            });
        }

        #endregion Methods
    }
}
