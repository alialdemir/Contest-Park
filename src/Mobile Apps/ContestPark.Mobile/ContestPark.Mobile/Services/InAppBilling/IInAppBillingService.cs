using ContestPark.Mobile.Models.InAppBillingProduct;
using Plugin.InAppBilling.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.InAppBilling
{
    public interface IInAppBillingService
    {
        Task<InAppBillingPurchase> ConsumePurchaseAsync(string productId, string purchaseToken);
        Task<List<InAppBillingProductModel>> GetProductInfoAsync();

        Task<InAppBillingPurchaseModel> PurchaseAsync(string productId);
    }
}
