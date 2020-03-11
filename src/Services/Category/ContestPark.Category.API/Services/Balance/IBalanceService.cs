using ContestPark.Category.API.Models;

namespace ContestPark.Category.API.Services.Balance
{
    public interface IBalanceService
    {
        System.Threading.Tasks.Task<BalanceModel> GetBalance(string userId, IntegrationEvents.Events.BalanceTypes balanceType);
    }
}