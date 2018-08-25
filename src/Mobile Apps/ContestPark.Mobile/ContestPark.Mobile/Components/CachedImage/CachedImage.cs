using ContestPark.Mobile.Helpers;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class CachedImage : FFImageLoading.Forms.CachedImage
    {
        public CachedImage()
        {
            RetryCount = 0;
            //Aspect = Aspect.AspectFill;
            DownsampleToViewSize = true;
            DownsampleUseDipUnits = false;
            AutomationId = "imgProfilePhoto";
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
            CacheDuration = new TimeSpan(0, 1, 0, 0);
            //CacheType = FFImageLoading.Cache.CacheType.Memory;
            LoadingPlaceholder = DefaultImages.DefaultProfilePicture;
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => Command?.Execute(CommandParameter);
            GestureRecognizers.Add(tapGestureRecognizer);
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(propertyName: nameof(Command),
                                                                                           returnType: typeof(ICommand),
                                                                                           declaringType: typeof(CircleImage),
                                                                                           defaultValue: null);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(propertyName: nameof(CommandParameter),
                                                                                                           returnType: typeof(object),
                                                                                                           declaringType: typeof(CircleImage),
                                                                                                           defaultValue: null);

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
    }
}