using FFImageLoading.Transformations;
using System.Linq;
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

                if (Transformations.Any(x => x.GetType() == typeof(CircleTransformation)))
                    Transformations.Remove(Transformations.FirstOrDefault(x => x.GetType() == typeof(CircleTransformation)));

                Transformations.Add(new CircleTransformation(10, value));
            }
        }

        protected override void OnBindingContextChanged()
        {
            if (WidthRequest < 0)
                WidthRequest = 50;

            if (HeightRequest < 0)
                HeightRequest = 50;

            if (!Transformations.Any(x => x.GetType() == typeof(CircleTransformation)))
            {
                Transformations.Add(new CircleTransformation(10, BorderColor));

                ReloadImage();
            }

            base.OnBindingContextChanged();
        }
    }
}
