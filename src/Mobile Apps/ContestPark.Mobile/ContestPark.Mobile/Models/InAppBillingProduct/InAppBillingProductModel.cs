﻿using ContestPark.Mobile.Models.Base;

namespace ContestPark.Mobile.Models.InAppBillingProduct
{
    public class InAppBillingProductModel : IModelBase
    {
        public string CurrencyCode { get; set; }
        public string Description { get; set; }

        public string DisplayPrice
        {
            get
            {
                return $"{CurrencyCode} {LocalizedPrice}";
            }
        }

        public string Image { get; set; }
        public string LocalizedPrice { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
    }
}