using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Media;
using Plugin.InAppBilling.Abstractions;

namespace ContestPark.Mobile.Models.Balance
{
    public class PurchaseModel : MediaModel
    {
        /// <summary>
        /// App store package name
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        ///  App store product id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Platform
        /// </summary>
        public Platforms Platform { get; set; }

        public PurchaseState State { get; set; }

        public string TransactionId { get; set; }
        public string VerifyPurchase { get; set; }
        public string Token { get; set; }
        public string Paylaod { get; set; }
    }
}
