using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Cp
{
    public interface ICpService
    {
        Task<int> GetTotalCpByUserIdAsync();

        Task<bool> PurchaseAsync(string productId);
    }
}