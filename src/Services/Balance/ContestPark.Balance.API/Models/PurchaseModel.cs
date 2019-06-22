using ContestPark.Balance.API.Enums;
using ContestPark.Balance.API.Resources;
using System.ComponentModel.DataAnnotations;

namespace ContestPark.Balance.API.Models
{
    public class PurchaseModel
    {
        /// <summary>
        /// App store package name
        /// </summary>
        [Required]
        public string PackageName { get; set; }

        /// <summary>
        ///  App store product id
        /// </summary>
        [Required]
        public string ProductId { get; set; }

        /// <summary>
        /// App store token
        /// </summary>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// Platform
        /// </summary>
        [Required]
        [EnumDataType(typeof(Platforms), ErrorMessageResourceType = typeof(BalanceResource), ErrorMessageResourceName = "InvalidPlatformType")]
        public Platforms Platform { get; set; }
    }
}