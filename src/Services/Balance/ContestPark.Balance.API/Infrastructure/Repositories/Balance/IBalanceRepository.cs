using ContestPark.Balance.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Infrastructure.Repositories.Balance
{
    public interface IBalanceRepository
    {
        BalanceModel GetUserBalances(string userId);

        Task<bool> UpdateBalanceAsync(ChangeBalanceModel changeBalance);
        bool WithdrawalStatus(string userId);
    }
}
