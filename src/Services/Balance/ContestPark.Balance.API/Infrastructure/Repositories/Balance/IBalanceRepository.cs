using ContestPark.Balance.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Infrastructure.Repositories.Balance
{
    public interface IBalanceRepository
    {
        IEnumerable<BalanceModel> GetUserBalances(string userId);

        Task<bool> ChangeBalanceByUserId(ChangeBalanceModel changeBalance);
    }
}