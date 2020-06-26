using ContestPark.Balance.API.Enums;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ContestPark.Balance.API.Models
{
    public class PurchaseModel
    {
        /// <summary>
        /// App store package name
        /// </summary>
        //[Required]
        public string PackageName { get; set; }

        /// <summary>
        ///  App store product id
        /// </summary>
        //[Required]
        public string ProductId { get; set; }

        /// <summary>
        /// App store token
        /// </summary>
        [JsonIgnore]
        public string Token { get; set; }

        /// <summary>
        /// Platform
        /// </summary>
        //[Required]
        //[EnumDataType(typeof(Platforms), ErrorMessageResourceType = typeof(BalanceResource), ErrorMessageResourceName = "InvalidPlatformType")]
        public Platforms Platform { get; set; }

        public IFormFile Files { get; set; }

        public PurchaseState State { get; set; }

        public string TransactionId { get; set; }

        public string VerifyPurchase { get; set; }
    }
}
