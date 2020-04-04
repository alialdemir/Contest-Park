using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class LinearGradientButton : Button
    {
        public static readonly BindableProperty StartColorProperty = BindableProperty.Create(nameof(StartColor),
                                                                                            typeof(Color),
                                                                                            typeof(LinearGradientButton),
                                                                                            defaultValue: Color.Default);

        // Gets or sets BorderWidth value
        public Color StartColor
        {
            get { return (Color)GetValue(StartColorProperty); }
            set { SetValue(StartColorProperty, value); }
        }

        public static readonly BindableProperty EndColorProperty = BindableProperty.Create(nameof(EndColor),
                                                                                           typeof(Color),
                                                                                           typeof(LinearGradientButton),
                                                                                           defaultValue: Color.Default);

        // Gets or sets BorderWidth value
        public Color EndColor
        {
            get { return (Color)GetValue(EndColorProperty); }
            set { SetValue(EndColorProperty, value); }
        }

        public static readonly BindableProperty IsUpperCaseProperty = BindableProperty.Create(nameof(IsUpperCase),
                                                                                           typeof(bool),
                                                                                           typeof(LinearGradientButton),
                                                                                           defaultValue: false);

        // Gets or sets IsUpperCase value
        public bool IsUpperCase
        {
            get { return (bool)GetValue(IsUpperCaseProperty); }
            set { SetValue(IsUpperCaseProperty, value); }
        }
    }
}
