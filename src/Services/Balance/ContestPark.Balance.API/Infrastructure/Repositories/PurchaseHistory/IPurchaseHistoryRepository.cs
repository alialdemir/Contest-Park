using System.Threading.Tasks;

namespace ContestPark.Balance.API.Infrastructure.Repositories.PurchaseHistory
{
    public interface IPurchaseHistoryRepository
    {
        Task<bool> AddAsync(Tables.PurchaseHistory purchase);
    }
}
