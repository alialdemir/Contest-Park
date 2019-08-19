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
                Gold = 999999.99m,
                Money = 9999912.99m
            });
        }

        public Task<bool> PurchaseAsync(PurchaseModel purchase)
        {
            return Task.FromResult(true);
        }
    }
}
