using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Base;
using ContestPark.Mobile.ViewModels.Base;
using System;
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
                return $"{LocalizedPrice} {CurrencyCode}";
            }
        }

        public string Image { get; set; }
        public string LocalizedPrice { get; set; }
        private string _productId;

        public string ProductId
        {
            get { return _productId; }
            set
            {
                _productId = value;
                RaisePropertyChanged(() => ProductId);
            }
        }

        public string ProductName { get; set; }
        public BalanceTypes BalanceTypes { get; set; }

        public string DiscountPrice
        {
            get
            {
                if (BalanceTypes == BalanceTypes.Money && LocalizedPrice.Equals("₺12,99"))//Eğer bakiye tipi para ise 12.99 tl olan ürüne en çok satılan diye yazı ekler
                    return ContestParkResources.BestSeller;

                if (string.IsNullOrEmpty(LocalizedPrice) || BalanceTypes == BalanceTypes.Money)
                    return string.Empty;

                decimal price = Convert.ToDecimal(LocalizedPrice.Replace("₺", "").Replace("$", ""));// Farklı para birimlerinde burası patlar

                return "₺" + ((price * 20 / 100) + price).ToString("N2");// Fiyatın %20 fazlası
            }
        }

        public Color RightText2TextColor
        {
            get
            {
                return BalanceTypes == BalanceTypes.Money
                    ? Color.FromHex("#ff8800")
                    : Color.Black;
            }
        }

        public TextDecorations RightText2TextDecorations
        {
            get
            {
                return BalanceTypes == BalanceTypes.Money
                    ? TextDecorations.None
                    : TextDecorations.Strikethrough;
            }
        }
    }
}
