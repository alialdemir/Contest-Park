using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.InAppBillingProduct;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.InAppBilling
{
    public class InAppBillingMockService : IInAppBillingService
    {
        public Task<List<InAppBillingProductModel>> GetProductInfoAsync()
        {
            List<InAppBillingProductModel> list = new List<InAppBillingProductModel>
            {
                new InAppBillingProductModel
                {
                     CurrencyCode= "TRY",
                     Description = "A small investment can go far!",
                     Image = "contest_store_money_1.svg",
                     LocalizedPrice="6.99",
                     ProductId = "com.contestparkapp.app.250coins",
                     ProductName = "250 Coins",
                     BalanceTypes = BalanceTypes.Money
                },
                new InAppBillingProductModel
                {
                     CurrencyCode= "TRY",
                     Description = "Nice chunk of coins you have over here",
                     Image = "contest_store_money_2.svg",
                     LocalizedPrice="34.99",
                     ProductId = "com.contestparkapp.app.1500coins",
                     ProductName = "1500 Coins",
                     BalanceTypes = BalanceTypes.Money
                },
                new InAppBillingProductModel
                {
                     CurrencyCode= "TRY",
                     Description = "Shake it, shale it like a wallet full of coins",
                     Image = "contest_store_money_3.svg",
                     LocalizedPrice="134.99",
                     ProductId = "com.contestparkapp.app.7000coins",
                     ProductName = "7000 Coins",
                     BalanceTypes = BalanceTypes.Money
                },
                new InAppBillingProductModel
                {
                     CurrencyCode= "TRY",
                     Description = "Coins like a cartoon duck",
                     Image = "contest_store_money_4.svg",
                     LocalizedPrice="349.99",
                     ProductId = "com.contestparkapp.app.20000coins",
                     ProductName = "20000 Coins",
                     BalanceTypes = BalanceTypes.Money
                },
                //

                new InAppBillingProductModel
                {
                     CurrencyCode= "TRY",
                     Description = "A small investment can go far!",
                     Image = "contest_store_gold_1.svg",
                     LocalizedPrice="6.99",
                     ProductId = "com.contestparkapp.app.250coins",
                     ProductName = "250 Coins",
                     BalanceTypes = BalanceTypes.Gold
                },
                new InAppBillingProductModel
                {
                     CurrencyCode= "TRY",
                     Description = "Nice chunk of coins you have over here",
                     Image = "contest_store_gold_2.svg",
                     LocalizedPrice="34.99",
                     ProductId = "com.contestparkapp.app.1500coins",
                     ProductName = "1500 Coins",
                     BalanceTypes = BalanceTypes.Gold
                },
                new InAppBillingProductModel
                {
                     CurrencyCode= "TRY",
                     Description = "Shake it, shale it like a wallet full of coins",
                     Image = "contest_store_gold_3.svg",
                     LocalizedPrice="134.99",
                     ProductId = "com.contestparkapp.app.7000coins",
                     ProductName = "7000 Coins",
                     BalanceTypes = BalanceTypes.Gold
                },
                new InAppBillingProductModel
                {
                     CurrencyCode= "TRY",
                     Description = "Coins like a cartoon duck",
                     Image = "contest_store_gold_4.svg",
                     LocalizedPrice="349.99",
                     ProductId = "com.contestparkapp.app.20000coins",
                     ProductName = "20000 Coins",
                     BalanceTypes = BalanceTypes.Gold
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
