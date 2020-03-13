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
            string sql = @"(SELECT (CASE
                            WHEN (
                            SELECT HOUR(TIMEDIFF(NOW(), bh.CreatedDate)) FROM BalanceHistories bh
                            WHERE bh.BalanceHistoryType  = @balanceHistoryType
                            AND bh.UserId = @userId
                            ORDER BY bh.CreatedDate DESC
                            LIMIT 0,1) >= 12
                            THEN 1
                            ELSE NULL
                            END))";

            bool? isReward = _balanceHistoryRepository.QuerySingleOrDefault<bool?>(sql, new
            {
                balanceHistoryType = (byte)BalanceHistoryTypes.DailyChip,
                userId,
            });

            if (isReward.HasValue)
                return isReward.Value;

            return true;
        }

        #endregion Methods
    }
}
