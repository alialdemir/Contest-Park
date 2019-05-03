using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Cp
{
    public class CpMockService : ICpService
    {
        public Task<int> GetTotalCpByUserIdAsync()
        {
            return Task.FromResult(100000);
        }

        public Task<bool> PurchaseAsync(string productId)
        {
            return Task.FromResult(true);
        }
    }
}