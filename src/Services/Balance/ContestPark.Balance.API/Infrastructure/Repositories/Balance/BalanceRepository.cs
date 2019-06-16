using ContestPark.Balance.API.Models;
using ContestPark.Core.CosmosDb.Interfaces;
using System.Collections.Generic;

namespace ContestPark.Balance.API.Infrastructure.Repositories.Cp
{
    public class BalanceRepository : IBalanceRepository
    {
        #region Private Variables

        private readonly IDocumentDbRepository<Documents.Balance> _balanceRepository;

        #endregion Private Variables

        #region Constructor

        public BalanceRepository(IDocumentDbRepository<Documents.Balance> balanceRepository)
        {
            _balanceRepository = balanceRepository;
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

            return _balanceRepository.QueryMultipleAsync<BalanceModel>(sql, new
            {
                userId
            });
        }

        #endregion Methods
    }
}