using ContestPark.Mobile.Models.InAppBillingProduct;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.InAppBilling
{
    public interface IInAppBillingService
    {
        Task<List<InAppBillingProductModel>> GetProductInfoAsync();

        Task<InAppBillingPurchaseModel> PurchaseAsync(string productId);
    }
}
