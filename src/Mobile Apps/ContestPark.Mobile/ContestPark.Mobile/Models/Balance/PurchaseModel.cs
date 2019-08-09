using ContestPark.Mobile.Enums;

namespace ContestPark.Mobile.Models.Balance
{
    public class PurchaseModel
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
        /// App store token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Platform
        /// </summary>
        public Platforms Platform { get; set; }
    }
}
