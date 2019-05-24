using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components.IconButton
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IconButton : ContentView
    {
        #region Constructor

        public IconButton()
        {
            InitializeComponent();
        }

        #endregion Constructor

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(propertyName: nameof(Command),
                                                                                                    returnType: typeof(ICommand),
                                                                                                    declaringType: typeof(IconButton),
                                                                                                    defaultValue: null);

        public static readonly BindableProperty IconColorProperty = BindableProperty.Create(propertyName: nameof(IconColor),
                                                                                                            returnType: typeof(Color),
                                                                                                    declaringType: typeof(IconButton),
                                                                                                    defaultValue: Color.Default);

        public static readonly BindableProperty IconProperty = BindableProperty.Create(propertyName: nameof(Icon),
                                                                                                    returnType: typeof(string),
                                                                                                    declaringType: typeof(IconButton),
                                                                                                    defaultValue: String.Empty);

        public static readonly BindableProperty ParameterProperty = BindableProperty.Create(propertyName: nameof(Parameter),
                                                                                                    returnType: typeof(object),
                                                                                                    declaringType: typeof(IconButton),
                                                                                                    defaultValue: null);

        public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: nameof(Text),
                                                                                                  returnType: typeof(string),
                                                                                                  declaringType: typeof(IconButton),
                                                                                                  defaultValue: String.Empty);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public Color IconColor
        {
            get { return (Color)GetValue(IconColorProperty); }
            set { SetValue(IconColorProperty, value); }
        }

        public object Parameter
        {
            get { return (object)GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}