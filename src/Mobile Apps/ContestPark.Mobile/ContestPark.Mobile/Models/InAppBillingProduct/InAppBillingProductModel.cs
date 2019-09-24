using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Base;
using ContestPark.Mobile.ViewModels.Base;

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
    }
}
