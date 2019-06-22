using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.Category.API.Model;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Services.Balance
{
    public interface IBalanceService
    {
        Task<BalanceModel> GetBalance(string userId, BalanceTypes balanceType);
    }
}