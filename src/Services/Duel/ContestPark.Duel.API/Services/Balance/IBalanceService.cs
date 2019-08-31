using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Services.Balance
{
    public interface IBalanceService
    {
        Task<BalanceModel> GetBalance(string userId, BalanceTypes balanceType);
    }
}
