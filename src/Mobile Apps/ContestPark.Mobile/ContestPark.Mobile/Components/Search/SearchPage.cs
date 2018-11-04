using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class SearchPage : ContentPage
    {
        public static readonly BindableProperty IsSearchFocusProperty = BindableProperty.Create(nameof(IsSearchFocus), typeof(bool), typeof(SearchPage), false);

        public bool IsSearchFocus
        {
            get
            {
                return (bool)GetValue(IsSearchFocusProperty);
            }
            set
            {
                SetValue(IsSearchFocusProperty, value);
            }
        }

        public static readonly BindableProperty SearchPlaceHolderTextProperty = BindableProperty.Create(nameof(SearchPlaceHolderText), typeof(string), typeof(SearchPage), string.Empty);

        public string SearchPlaceHolderText
        {
            get
            {
                return (string)GetValue(SearchPlaceHolderTextProperty);
            }
            set
            {
                SetValue(SearchPlaceHolderTextProperty, value);
            }
        }

        public static readonly BindableProperty SearchPlaceHolderColorProperty = BindableProperty.Create(nameof(SearchPlaceHolderColor), typeof(Color), typeof(SearchPage), Color.FromHex("#fff"));

        public Color SearchPlaceHolderColor
        {
            get
            {
                return (Color)GetValue(SearchPlaceHolderColorProperty);
            }
            set
            {
                SetValue(SearchPlaceHolderColorProperty, value);
            }
        }

        public static readonly BindableProperty SeachTextColorProperty = BindableProperty.Create(nameof(SeachTextColor), typeof(Color), typeof(SearchPage), Color.FromHex("#fff"));

        public Color SeachTextColor
        {
            get
            {
                return (Color)GetValue(SeachTextColorProperty);
            }
            set
            {
                SetValue(SeachTextColorProperty, value);
            }
        }

        public static readonly BindableProperty SearchTextProperty = BindableProperty.Create(nameof(SearchText), typeof(string), typeof(SearchPage), string.Empty);

        public string SearchText
        {
            get
            {
                return (string)GetValue(SearchTextProperty);
            }
            set
            {
                SetValue(SearchTextProperty, value);
            }
        }

        public static readonly BindableProperty SearchCommandProperty = BindableProperty.Create(nameof(SearchCommand), typeof(ICommand), typeof(SearchPage));

        public ICommand SearchCommand
        {
            get
            {
                return (ICommand)GetValue(SearchCommandProperty);
            }
            set
            {
                SetValue(SearchCommandProperty, value);
            }
        }

        public static readonly BindableProperty SearchIconColorProperty = BindableProperty.Create(nameof(SearchIconColor), typeof(Color), typeof(SearchPage), Color.FromHex("#fff"));

        public Color SearchIconColor
        {
            get
            {
                return (Color)GetValue(SearchIconColorProperty);
            }
            set
            {
                SetValue(SearchIconColorProperty, value);
            }
        }

        public static readonly BindableProperty SearchCloseIconColorProperty = BindableProperty.Create(nameof(SearchCloseIconColor), typeof(Color), typeof(SearchPage), Color.FromHex("#fff"));

        public Color SearchCloseIconColor
        {
            get
            {
                return (Color)GetValue(SearchCloseIconColorProperty);
            }
            set
            {
                SetValue(SearchCloseIconColorProperty, value);
            }
        }
    }
}