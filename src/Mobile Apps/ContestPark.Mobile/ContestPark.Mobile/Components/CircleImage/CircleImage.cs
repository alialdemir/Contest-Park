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
            if (WidthRequest < 0)
                WidthRequest = 50;

            if (HeightRequest < 0)
                HeightRequest = 50;

            Transformations.Add(new CircleTransformation(10, BorderColor));

            base.OnBindingContextChanged();
        }
    }
}
