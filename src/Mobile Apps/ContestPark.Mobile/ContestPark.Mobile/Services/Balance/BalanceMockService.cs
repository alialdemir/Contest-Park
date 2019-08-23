using ContestPark.Mobile.Models.Balance;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Cp
{
    public class BalanceMockService : IBalanceService
    {
        public Task<BalanceModel> GetTotalCpByUserIdAsync()
        {
            return Task.FromResult(new BalanceModel
            {
                Gold = 34543,
                Money = 54.12m
            });
        }

        public Task<bool> PurchaseAsync(PurchaseModel purchase)
        {
            return Task.FromResult(true);
        }
    }
}
