using ContestPark.Balance.API.Models;
using System.Collections.Generic;

namespace ContestPark.Balance.API.Infrastructure.Repositories.Cp
{
    public interface IBalanceRepository
    {
        IEnumerable<BalanceModel> GetUserBalances(string userId);
    }
}