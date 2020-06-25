using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class AdMobView : View
    {
        public static readonly BindableProperty AdUnitIdProperty = BindableProperty.Create(
                      nameof(AdUnitId),
                      typeof(string),
                      typeof(AdMobView),
                      string.Empty);

        public string AdUnitId
        {
            get => (string)GetValue(AdUnitIdProperty);
            set => SetValue(AdUnitIdProperty, value);
        }

        public static readonly BindableProperty UserPersonalizedAdsProperty = BindableProperty.Create(
                      nameof(UserPersonalizedAds),
                      typeof(bool),
                      typeof(AdMobView),
                      true);

        public bool UserPersonalizedAds
        {
            get => (bool)GetValue(UserPersonalizedAdsProperty);
            set => SetValue(UserPersonalizedAdsProperty, value);
        }
    }
}
