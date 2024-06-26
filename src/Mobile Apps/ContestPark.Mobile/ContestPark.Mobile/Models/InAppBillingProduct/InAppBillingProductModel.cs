﻿using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Base;
using ContestPark.Mobile.ViewModels.Base;
using Xamarin.Forms;

namespace ContestPark.Mobile.Models.InAppBillingProduct
{
    public class InAppBillingProductModel : ExtendedBindableObject, IModelBase
    {
        public string CurrencyCode { get; set; }
        public string Description { get; set; }

        public string DisplayPrice
        {
            get
            {
                return $"{LocalizedPrice.Replace("TRY", "").Replace("₺", "").Replace("$", "").Trim()}{CurrencyCode.Replace("TRY", "₺")}";
            }
        }

        public string DisplaySpecialDescription
        {
            get
            {
                return LocalizedPrice.Replace("TRY", "").Replace("₺", "").Replace("$", "").Trim();
            }
        }

        public string Image { get; set; }
        public string LocalizedPrice { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }

        public BalanceTypes BalanceTypes { get; set; }

        public string DiscountPrice { get; set; }

        public Color RightText2TextColor { get; set; }
        public TextDecorations RightText2TextDecorations { get; set; }
        public bool IsSpecialOffer { get; set; }
        public decimal DiscountBalanceAmount { get; set; }
    }
}
