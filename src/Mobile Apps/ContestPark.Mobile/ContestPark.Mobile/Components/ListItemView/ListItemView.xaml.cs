using ContestPark.Mobile.GestureRecognizer;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListItemView : ContentView
    {
        #region Constructors

        public ListItemView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(propertyName: nameof(Source),
                                                                                                    returnType: typeof(string),
                                                                                                    declaringType: typeof(ListItemView),
                                                                                                    defaultValue: String.Empty);

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: nameof(Text),
                                                                                                    returnType: typeof(string),
                                                                                                    declaringType: typeof(ListItemView),
                                                                                                    defaultValue: String.Empty);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty DetailProperty = BindableProperty.Create(propertyName: nameof(Detail),
                                                                                                    returnType: typeof(string),
                                                                                                    declaringType: typeof(ListItemView),
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
                                                                                                    declaringType: typeof(ListItemView),
                                                                                                    defaultValue: String.Empty);

        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set
            {
                SetValue(UserNameProperty, value);
            }
        }

        public static readonly BindableProperty DateProperty = BindableProperty.Create(propertyName: nameof(Date),
                                                                                                    returnType: typeof(string),
                                                                                                    declaringType: typeof(ListItemView),
                                                                                                    defaultValue: String.Empty);

        public string Date
        {
            get { return (string)GetValue(DateProperty); }
            set
            {
                SetValue(DateProperty, value);
            }
        }

        public static readonly BindableProperty LongPressedProperty = BindableProperty.Create(propertyName: nameof(LongPressed),
                                                                                                      returnType: typeof(ICommand),
                                                                                                      declaringType: typeof(ListItemView),
                                                                                                      defaultValue: null);

        public ICommand LongPressed
        {
            get { return (ICommand)GetValue(LongPressedProperty); }
            set
            {
                SetValue(LongPressedProperty, value);
            }
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(propertyName: nameof(CommandParameter),
                                                                                                           returnType: typeof(object),
                                                                                                           declaringType: typeof(ListItemView),
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
                                                                                                    declaringType: typeof(ListItemView),
                                                                                                    defaultValue: null);

        public ICommand SingleTap
        {
            get { return (ICommand)GetValue(SingleTapProperty); }
            set
            {
                SetValue(SingleTapProperty, value);
            }
        }

        public static readonly BindableProperty GotoProfilePageCommandProperty = BindableProperty.Create(propertyName: nameof(GotoProfilePageCommand),
                                                                                                    returnType: typeof(ICommand),
                                                                                                    declaringType: typeof(ListItemView),
                                                                                                    defaultValue: null);

        public ICommand GotoProfilePageCommand
        {
            get { return (ICommand)GetValue(GotoProfilePageCommandProperty); }
            set
            {
                SetValue(GotoProfilePageCommandProperty, value);
            }
        }

        #endregion Properties

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            cstmGrid.GestureRecognizers.Add(new LongPressGestureRecognizer
            {
                Command = LongPressed,
                CommandParameter = CommandParameter
            });

            cstmGrid.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = LongPressed,
                CommandParameter = CommandParameter
            });
        }

        #endregion Override
    }
}
