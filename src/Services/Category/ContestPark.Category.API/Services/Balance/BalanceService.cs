using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.Category.API.Models;
using ContestPark.Core.Services.RequestProvider;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Services.Balance
{
    public class BalanceService : IBalanceService
    {
        private readonly IRequestProvider _requestProvider;

        private readonly CategorySettings _categorySettings;

        public BalanceService(IRequestProvider requestProvider,
                              IOptions<CategorySettings> settings)
        {
            _requestProvider = requestProvider ?? throw new ArgumentNullException(nameof(requestProvider));
            _categorySettings = settings.Value;
        }

        public Task<BalanceModel> GetBalance(string userId, BalanceTypes balanceType = BalanceTypes.Gold)
        {
            return _requestProvider.GetAsync<BalanceModel>($"{_categorySettings.BalanceUrl}/api/v1/Balance/{userId}?balanceType={balanceType}");
        }
    }
}
