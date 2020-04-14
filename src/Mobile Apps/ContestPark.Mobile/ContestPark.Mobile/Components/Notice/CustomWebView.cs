using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class CustomWebView : WebView
    {
        public static readonly BindableProperty PageTitleProperty = BindableProperty.Create(propertyName: nameof(PageTitle),
                                                                                                                        returnType: typeof(string),
                                                                                                                        defaultBindingMode: BindingMode.OneWayToSource,
                                                                                                                        declaringType: typeof(CustomWebView),
                                                                                                                        defaultValue: string.Empty);

        /// <summary>
        /// Açılan Sayfanın bağlığı
        /// </summary>
        public string PageTitle
        {
            get { return (string)GetValue(PageTitleProperty); }
            set { SetValue(PageTitleProperty, value); }
        }
    }
}
