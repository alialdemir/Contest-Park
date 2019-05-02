using ContestPark.Mobile.Models.InAppBillingProduct;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.InAppBilling
{
    public class InAppBillingMockService : IInAppBillingService
    {
        public Task<IEnumerable<InAppBillingProductModel>> GetProductInfoAsync()
        {
            IEnumerable<InAppBillingProductModel> list = new List<InAppBillingProductModel>
            {
                new InAppBillingProductModel
                {
                     CurrencyCode= "TRY",
                     Description = "A small investment can go far!",
                     Image = "assets/images/gold_01.png",
                     LocalizedPrice="6.99",
                     ProductId = "com.contestparkapp.app.250coins",
                     ProductName = "250 Coins"
                },
                new InAppBillingProductModel
                {
                     CurrencyCode= "TRY",
                     Description = "Nice chunk of coins you have over here",
                     Image = "assets/images/gold_02.png",
                     LocalizedPrice="34.99",
                     ProductId = "com.contestparkapp.app.1500coins",
                     ProductName = "1500 Coins"
                },
                new InAppBillingProductModel
                {
                     CurrencyCode= "TRY",
                     Description = "Shake it, shale it like a wallet full of coins",
                     Image = "assets/images/gold_03.png",
                     LocalizedPrice="134.99",
                     ProductId = "com.contestparkapp.app.7000coins",
                     ProductName = "7000 Coins"
                },
                new InAppBillingProductModel
                {
                     CurrencyCode= "TRY",
                     Description = "Coins like a cartoon duck",
                     Image = "assets/images/gold_04.png",
                     LocalizedPrice="349.99",
                     ProductId = "com.contestparkapp.app.20000coins",
                     ProductName = "20000 Coins"
                },
            };

            return Task.FromResult(list);
        }

        public Task<InAppBillingPurchaseModel> PurchaseAsync(string productId)
        {
            return Task.FromResult(new InAppBillingPurchaseModel
            {
                Id = Guid.NewGuid().ToString(),
                AutoRenewing = false,
                Payload = "payload",
                ProductId = productId,
                PurchaseToken = Guid.NewGuid().ToString(),
                TransactionDateUtc = DateTime.UtcNow
            });
        }
    }
}