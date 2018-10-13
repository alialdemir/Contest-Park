using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListItemButtonView : ContentView
    {
        #region Constructors

        public ListItemButtonView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(propertyName: nameof(Source),
                                                                                                    returnType: typeof(string),
                                                                                                    declaringType: typeof(ListItemButtonView),
                                                                                                    defaultValue: String.Empty);

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: nameof(Text),
                                                                                                    returnType: typeof(string),
                                                                                                    declaringType: typeof(ListItemButtonView),
                                                                                                    defaultValue: String.Empty);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty DetailProperty = BindableProperty.Create(propertyName: nameof(Detail),
                                                                                                    returnType: typeof(string),
                                                                                                    declaringType: typeof(ListItemButtonView),
                                                                                                    defaultValue: String.Empty);

        public string Detail
        {
            get { return (string)GetValue(DetailProperty); }
            set
            {
                SetValue(DetailProperty, value);
            }
        }

        public static readonly BindableProperty UserNameProperty = BindableProperty.Create(propertyName: nameof(UserName),
                                                                                                    returnType: typeof(string),
                                                                                                    declaringType: typeof(ListItemButtonView),
                                                                                                    defaultValue: String.Empty);

        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set
            {
                SetValue(UserNameProperty, value);
            }
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(propertyName: nameof(CommandParameter),
                                                                                                           returnType: typeof(object),
                                                                                                           declaringType: typeof(ListItemButtonView),
                                                                                                           defaultValue: null);

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        public static readonly BindableProperty SingleTapProperty = BindableProperty.Create(propertyName: nameof(SingleTap),
                                                                                                    returnType: typeof(ICommand),
                                                                                                    declaringType: typeof(ListItemButtonView),
                                                                                                    defaultValue: null);

        public ICommand SingleTap
        {
            get { return (ICommand)GetValue(SingleTapProperty); }
            set
            {
                SetValue(SingleTapProperty, value);
            }
        }

        public static readonly BindableProperty IconProperty = BindableProperty.Create(propertyName: nameof(Icon),
                                                                                                    returnType: typeof(string),
                                                                                                    declaringType: typeof(ListItemButtonView),
                                                                                                    defaultValue: String.Empty);

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public static readonly BindableProperty IsIconStatusProperty = BindableProperty.Create(propertyName: nameof(IsIconStatus),
                                                                                                    returnType: typeof(bool),
                                                                                                    declaringType: typeof(ListItemButtonView),
                                                                                                    defaultValue: true);

        public bool IsIconStatus
        {
            get { return (bool)GetValue(IsIconStatusProperty); }
            set
            {
                SetValue(IsIconStatusProperty, value);
            }
        }

        #endregion Properties

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            btnRight.Text = IsIconStatus ? Icon.Replace("fas", "far") : Icon.Replace("far", "fas");
            btnRight.Command = SingleTap;
            btnRight.CommandParameter = CommandParameter;
        }

        #endregion Override
    }
}