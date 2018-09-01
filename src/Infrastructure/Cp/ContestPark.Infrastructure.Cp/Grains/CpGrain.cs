using ContestPark.Domain.Cp.Enums;
using ContestPark.Domain.Cp.Interfaces;
using ContestPark.Infrastructure.Cp.Repositories.Cp;
using Microsoft.Extensions.Logging;
using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Cp.Grains
{
    public class CpGrain : Grain, ICpGrain
    {
        #region Private variables

        private ICpRepository _cpRepository;
        private readonly ILogger<CpGrain> _logger;

        #endregion Private variables

        #region Constructor

        public CpGrain(ICpRepository cpRepository,
                       ILogger<CpGrain> logger)
        {
            _cpRepository = cpRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı Id'sine göre toplam altın miktarını verir
        /// </summary>
        /// <param name="userId">Kullanıcı Id</param>
        /// <returns>Toplam altın miktarı</returns>
        public Task<int> GetTotalGoldByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return Task.FromResult(0);

            return Task.FromResult(_cpRepository.GetTotalGoldByUserId(userId));
        }

        /// <summary>
        /// Kullanıcının istenilen miktarda altınını azaltır -1 dönerse sorun vardır
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="diminishingGold">Azaltmak istenilen miktar</param>
        /// <param name="goldProcessName">Hangi sebepten azaltılıyor</param>
        /// <returns>Azaldıktan sonraki altın miktarı</returns>
        public Task<int> RemoveGold(string userId, int diminishingGold, GoldProcessNames goldProcessName)
        {
            if (string.IsNullOrEmpty(userId) || diminishingGold <= 0)
            {
                _logger.LogWarning($"Kullanıcı id boş veya azaltılacak bahis miktarı negatif değer. userId: {userId} gold: {diminishingGold}");

                return Task.FromResult(-1);
            }

            if (userId.Contains("-bot"))
                return Task.FromResult(0);

            int removeGold = _cpRepository.RemoveGold(userId, diminishingGold, goldProcessName);

            return Task.FromResult(removeGold);
        }

        /// <summary>
        /// Kullanıcıya altın ekler
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="addedChips">Eklenecek altın miktarı</param>
        /// <param name="goldProcessName">Hangi sebepten artıyor</param>
        /// <returns>Azaldıktan sonraki altın miktarı</returns>
        public Task<int> AddGold(string userId, int diminishingGold, GoldProcessNames goldProcessName)
        {
            if (string.IsNullOrEmpty(userId) || diminishingGold <= 0)
            {
                _logger.LogWarning($"Kullanıcı id boş veya azaltılacak bahis miktarı negatif değer. userId: {userId} gold: {diminishingGold}");

                return Task.FromResult(-1);
            }

            if (userId.Contains("-bot"))
                return Task.FromResult(0);

            int addedGold = _cpRepository.AddGold(userId, diminishingGold, goldProcessName);

            return Task.FromResult(addedGold);
        }

        #endregion Methods
    }
}