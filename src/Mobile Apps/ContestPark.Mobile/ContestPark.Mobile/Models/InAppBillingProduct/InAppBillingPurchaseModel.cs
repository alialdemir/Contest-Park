using Plugin.InAppBilling.Abstractions;
using System;

namespace ContestPark.Mobile.Models.InAppBillingProduct
{
    public class InAppBillingPurchaseModel
    {
        //
        // Summary:
        //     Indicates whether the subscritpion renewes automatically. If true, the sub is
        //     active, else false the user has canceled.
        public bool AutoRenewing { get; set; }

        public ConsumptionState ConsumptionState { get; set; }

        //
        // Summary:
        //     Purchase/Order Id
        public string Id { get; set; }

        //
        // Summary:
        //     Developer payload
        public string Payload { get; set; }

        //
        // Summary:
        //     Product Id/Sku
        public string ProductId { get; set; }

        //
        // Summary:
        //     Unique token identifying the purchase for a given item
        public string PurchaseToken { get; set; }

        public PurchaseState State { get; set; }

        //
        // Summary:
        //     Trasaction date in UTC
        public DateTime TransactionDateUtc { get; set; }
    }
}