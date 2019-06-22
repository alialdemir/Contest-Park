using System.Threading.Tasks;

namespace ContestPark.Balance.API.Infrastructure.Repositories.Purchase
{
    public interface IPurchaseHistoryRepository
    {
        Task<bool> AddAsync(Documents.PurchaseHistory purchase);
    }
}