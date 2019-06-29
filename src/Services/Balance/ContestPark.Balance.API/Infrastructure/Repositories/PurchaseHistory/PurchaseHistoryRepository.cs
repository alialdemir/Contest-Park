using ContestPark.Core.Database.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Infrastructure.Repositories.Purchase
{
    public class PurchaseHistoryRepository : IPurchaseHistoryRepository
    {
        #region Private Variables

        private readonly IRepository<Documents.PurchaseHistory> _purchaseHistoryRepository;
        private readonly ILogger<PurchaseHistoryRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public PurchaseHistoryRepository(IRepository<Documents.PurchaseHistory> purchaseHistoryRepository,
                                 ILogger<PurchaseHistoryRepository> logger)
        {
            _purchaseHistoryRepository = purchaseHistoryRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Satın alma geçmişi ekler
        /// </summary>
        /// <param name="purchase">Satın alma bilgisi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> AddAsync(Documents.PurchaseHistory purchase)
        {
            _logger.LogInformation("Satın alma işlemi gerçekleşti",
                                   purchase.UserId,
                                   purchase.Amount,
                                   purchase.BalanceType,
                                   purchase.PackageName,
                                   purchase.ProductId,
                                   purchase.Token);

            bool issuccsess = await _purchaseHistoryRepository.AddAsync(purchase);

            if (!issuccsess)
            {
                _logger.LogCritical("CRITTICAL: Satın alma geçmişi eklenirken hata oluştu", purchase.UserId);
            }
            return issuccsess;
        }

        #endregion Methods
    }
}
