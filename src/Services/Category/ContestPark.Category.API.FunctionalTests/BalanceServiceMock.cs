using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.Category.API.Models;
using ContestPark.Category.API.Services.Balance;
using System.Threading.Tasks;

namespace ContestPark.Category.API.FunctionalTests
{
    public class BalanceServiceMock : IBalanceService
    {
        public Task<BalanceModel> GetBalance(string userId, BalanceTypes balanceType)
        {
            return Task.FromResult(new BalanceModel
            {
                Amount = 9999999
            });
        }
    }
}