using ContestPark.Mobile.Models.InAppBillingProduct;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.InAppBilling
{
    public interface IInAppBillingService
    {
        string SpecialProductId { get; }

        Task<InAppBillingProductModel> GetProductById(string productId);

        Task<List<InAppBillingProductModel>> GetProductInfoAsync();

        Task<InAppBillingPurchaseModel> PurchaseAsync(string productId);

        Task PurchaseProcessAsync(string productId);
    }
}
