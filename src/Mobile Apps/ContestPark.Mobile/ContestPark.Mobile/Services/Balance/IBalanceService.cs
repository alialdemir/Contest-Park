using ContestPark.Mobile.Models.Balance;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Cp
{
    public interface IBalanceService
    {
        Task<BalanceModel> GetTotalCpByUserIdAsync();

        Task<bool> PurchaseAsync(PurchaseModel purchase);
    }
}
