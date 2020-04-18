namespace ContestPark.Balance.API.Enums
{
    //
    // Summary:
    //     Gets the current status of the purchase
    public enum PurchaseState
    {
        //
        // Summary:
        //     Purchased and in good standing
        Purchased = 0,

        //
        // Summary:
        //     Purchase was canceled
        Canceled = 1,

        //
        // Summary:
        //     Purchase was refunded
        Refunded = 2,

        //
        // Summary:
        //     In the process of being processed
        Purchasing = 3,

        //
        // Summary:
        //     Transaction has failed
        Failed = 4,

        //
        // Summary:
        //     Was restored.
        Restored = 5,

        //
        // Summary:
        //     In queue, pending external action
        Deferred = 6,

        //
        // Summary:
        //     In free trial
        FreeTrial = 7,

        //
        // Summary:
        //     Pending Purchase
        PaymentPending = 8,

        //
        // Summary:
        //     Purchase state unknown
        Unknown = 9
    }
}
