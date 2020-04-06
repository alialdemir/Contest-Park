using ContestPark.Mobile.Models.Balance;
using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Cp
{
    public class BalanceMockService : IBalanceService
    {
        public Task<bool> GetBalanceRequest(IbanNoModel ibanNo)
        {
            return Task.FromResult(true);
        }

        public Task<BalanceModel> GetBalanceAsync()
        {
            return Task.FromResult(new BalanceModel
            {
                Gold = 34543,
                Money = 514.12m
            });
        }

        public Task<bool> PurchaseAsync(PurchaseModel purchase)
        {
            return Task.FromResult(true);
        }

        public Task<bool> BalanceCode(BalanceCodeModel balanceCodeModel)
        {
            return Task.FromResult(true);
        }

        public Task<bool> RewardedVideoaAsync()
        {
            return Task.FromResult(true);
        }

        public Task<RewardModel> RewardAsync()
        {
            return Task.FromResult(new RewardModel
            {
                Amount = 555.00m,
                NextRewardTime = TimeSpan.FromDays(365)
            });
        }
    }
}
