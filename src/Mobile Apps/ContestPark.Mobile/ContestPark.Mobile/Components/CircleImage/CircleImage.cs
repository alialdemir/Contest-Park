using FFImageLoading.Transformations;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class CircleImage : CachedImage
    {
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(propertyName: nameof(BorderColor),
                                                                                                           returnType: typeof(string),
                                                                                                           declaringType: typeof(CircleImage),
                                                                                                           defaultValue: "#FFC200");

        public string BorderColor
        {
            get { return (string)GetValue(BorderColorProperty); }
            set
            {
                SetValue(BorderColorProperty, value);
            }
        }

        protected override void OnBindingContextChanged()
        {
            if (WidthRequest < 0) WidthRequest = 60;
            if (HeightRequest < 0) HeightRequest = 60;
            Transformations.Add(new CircleTransformation(20, BorderColor));
            base.OnBindingContextChanged();
        }
    }
}