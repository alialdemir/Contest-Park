using ContestPark.Mobile.Models.Balance;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Cp
{
    public interface IBalanceService
    {
        Task<bool> GetBalanceRequest(IbanNoModel ibanNo);

        Task<BalanceModel> GetBalanceAsync();

        Task<bool> PurchaseAsync(PurchaseModel purchase);

        Task<bool> BalanceCode(BalanceCodeModel balanceCodeModel);

        Task<bool> RewardedVideoaAsync();

        Task<RewardModel> RewardAsync();
    }
}
