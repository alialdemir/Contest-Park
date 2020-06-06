using ContestPark.Admin.API.Model.Bet;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.Bet
{
    public class BetRepository : IBetRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Bet> _betRepository;

        #endregion Private Variables

        #region Constructor

        public BetRepository(IRepository<Tables.Bet> betRepository)
        {
            _betRepository = betRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Bahis ekleme
        /// </summary>
        /// <param name="bet">Bahis bilgileri</param>
        /// <returns>Başarılı ise true başarısız ise false döner</returns>
        public async Task<bool> AddAsync(Tables.Bet bet)
        {
            int? betId = await _betRepository.AddAsync(bet);

            return betId.HasValue && betId.Value > 0;
        }

        /// <summary>
        /// Silme işlemi
        /// </summary>
        /// <param name="betId">Bahis id</param>
        /// <returns>Başarılı ise true başarısız ise false döner</returns>
        public Task<bool> ClearAsync(byte betId)
        {
            return _betRepository.RemoveAsync(betId);
        }

        /// <summary>
        /// Bahis listesini döndürür
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Bahis listesi</returns>
        public ServiceModel<BetModel> GetBetList(PagingModel pagingModel)
        {
            string sql = "SELECT * FROM Bets";

            return _betRepository.ToServiceModel<BetModel>(sql, pagingModel: pagingModel);
        }

        /// <summary>
        /// Bahis güncelle
        /// </summary>
        /// <param name="betUpdateModel">Güncellenen bahis bilgileri</param>
        /// <returns>Başarılı ise true başarısız ise false döner</returns>
        public Task<bool> UpdateAsync(BetUpdateModel betUpdateModel)
        {
            return _betRepository.UpdateAsync(new Tables.Bet
            {
                BalanceType = betUpdateModel.BalanceType,
                BetId = betUpdateModel.BetId,
                Description = betUpdateModel.Description,
                EarnedCoin = betUpdateModel.EarnedCoin,
                EntryFee = betUpdateModel.EntryFee,
                Title = betUpdateModel.Title,
            });
        }

        #endregion Methods
    }
}
