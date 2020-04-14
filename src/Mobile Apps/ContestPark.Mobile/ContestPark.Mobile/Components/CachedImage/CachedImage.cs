using ContestPark.Mobile.Helpers;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class CachedImage : FFImageLoading.Forms.CachedImage
    {
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(propertyName: nameof(CommandParameter),
                                                                                                           returnType: typeof(object),
                                                                                                           declaringType: typeof(CircleImage),
                                                                                                           defaultValue: null);

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(propertyName: nameof(Command),
                                                                                           returnType: typeof(ICommand),
                                                                                           declaringType: typeof(CircleImage),
                                                                                           defaultValue: null);

        public static readonly BindableProperty IsLoadingPlaceholderProperty = BindableProperty.Create(propertyName: nameof(IsLoadingPlaceholder),
                                                                                           returnType: typeof(bool),
                                                                                           declaringType: typeof(CircleImage),
                                                                                           defaultValue: false);

        public CachedImage()
        {
            RetryCount = 0;
            DownsampleToViewSize = false;
            DownsampleUseDipUnits = false;
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
            CacheDuration = new TimeSpan(0, 1, 0, 0);
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => Command?.Execute(CommandParameter);
            GestureRecognizers.Add(tapGestureRecognizer);
        }

        protected override void OnBindingContextChanged()
        {
            if (IsLoadingPlaceholder)
            {
                LoadingPlaceholder = ImageSource.FromUri(new Uri(DefaultImages.DefaultProfilePicture));
            }

            base.OnBindingContextChanged();
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        public bool IsLoadingPlaceholder
        {
            get { return (bool)GetValue(IsLoadingPlaceholderProperty); }
            set
            {
                SetValue(IsLoadingPlaceholderProperty, value);
            }
        }
    }
}
