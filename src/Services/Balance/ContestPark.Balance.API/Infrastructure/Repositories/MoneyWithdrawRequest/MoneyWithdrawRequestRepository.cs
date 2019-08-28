using ContestPark.Core.Database.Interfaces;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Infrastructure.Repositories.MoneyWithdrawRequest
{
    public class MoneyWithdrawRequestRepository : IMoneyWithdrawRequestRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.MoneyWithdrawRequest> _moneyWithdrawRequestRepository;

        #endregion Private Variables

        #region Constructor

        public MoneyWithdrawRequestRepository(IRepository<Tables.MoneyWithdrawRequest> moneyWithdrawRequestRepository)
        {
            _moneyWithdrawRequestRepository = moneyWithdrawRequestRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Bakiye çekme talebi ekler
        /// </summary>
        /// <param name="money">Iban no ve ad soyad</param>
        public Task<bool> Insert(Tables.MoneyWithdrawRequest money)
        {
            return _moneyWithdrawRequestRepository.AddAsync(money);
        }

        #endregion Methods
    }
}
