using ContestPark.Core.Services.RequestProvider;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Models;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Services.Balance
{
    public class BalanceService : IBalanceService
    {
        #region Private variables

        private readonly IRequestProvider _requestProvider;

        private readonly DuelSettings _categorySettings;

        #endregion Private variables

        #region Constructor

        public BalanceService(IRequestProvider requestProvider,
                      IOptions<DuelSettings> settings)
        {
            _requestProvider = requestProvider ?? throw new ArgumentNullException(nameof(requestProvider));
            _categorySettings = settings.Value;
        }

        #endregion Constructor

        #region Methods

        public Task<BalanceModel> GetBalance(string userId, BalanceTypes balanceType)
        {
            return _requestProvider.GetAsync<BalanceModel>($"{_categorySettings.BalanceUrl}/api/v1/Balance/{userId}?balanceType={(byte)balanceType}");
        }

        #endregion Methods
    }
}
