using ContestPark.Mobile.Components.ContestStore;
using ContestPark.Mobile.Models.InAppBillingProduct;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class ContestStoreFactory : ContentView
    {
        public static readonly BindableProperty SingleTapProperty = BindableProperty.Create(propertyName: nameof(SingleTap),
                                                                                            returnType: typeof(ICommand),
                                                                                            declaringType: typeof(ContestStoreFactory),
                                                                                            defaultValue: null);

        public ICommand SingleTap
        {
            get { return (ICommand)GetValue(SingleTapProperty); }
            set
            {
                SetValue(SingleTapProperty, value);
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            InAppBillingProductModel product = (InAppBillingProductModel)BindingContext;
            if (product == null)
                return;

            if (product.IsSpecialOffer)
            {
                Content = new SpecialOfferView
                {
                    CommandParameter = product.ProductId,
                    Detail = product.Description,
                    IsShowRightText = true,
                    RightIconTextColor = Color.FromHex("#F7416E"),
                    RightText = product.DisplayPrice,
                    RightText2 = product.DiscountPrice,
                    RightText2TextColor = product.RightText2TextColor,
                    RightText2TextDecorations = product.RightText2TextDecorations,
                    SingleTap = SingleTap,
                    Source = product.Image,
                    Text = product.ProductName,
                    BindingContext = BindingContext
                };
            }
            else
            {
                Content = new ThumListItem
                {
                    CommandParameter = product.ProductId,
                    Detail = product.Description,
                    IsShowRightText = true,
                    RightIconTextColor = Color.FromHex("#F7416E"),
                    RightText = product.DisplayPrice,
                    RightText2 = product.DiscountPrice,
                    RightText2TextColor = product.RightText2TextColor,
                    RightText2TextDecorations = product.RightText2TextDecorations,
                    SingleTap = SingleTap,
                    Source = product.Image,
                    Text = product.ProductName,
                    BindingContext = BindingContext
                };
            }
        }
    }
}
