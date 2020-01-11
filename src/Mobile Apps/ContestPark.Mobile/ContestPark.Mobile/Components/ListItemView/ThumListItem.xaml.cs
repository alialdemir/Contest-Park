using ContestPark.Mobile.GestureRecognizer;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThumListItem : ContentView
    {
        #region Constructors

        public ThumListItem()
        {
            InitializeComponent();
            //BackgroundColor = (Color)ContestParkApp.Current.Resources["ListBackgroundColor"];
        }

        #endregion Constructors

        #region Properties

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(propertyName: nameof(CommandParameter),
                                                                                                   returnType: typeof(string),
                                                                                                   declaringType: typeof(ThumListItem),
                                                                                                   defaultValue: string.Empty);

        public static readonly BindableProperty DetailProperty = BindableProperty.Create(propertyName: nameof(Detail),
                                                                                         returnType: typeof(string),
                                                                                         declaringType: typeof(ThumListItem),
                                                                                         defaultValue: String.Empty);

        public static readonly BindableProperty GridBackgroundColorProperty = BindableProperty.Create(propertyName: nameof(GridBackgroundColor),
                                                                                                      returnType: typeof(Color),
                                                                                                      declaringType: typeof(ThumListItem),
                                                                                                      defaultValue: (Color)ContestParkApp.Current.Resources["ListBackgroundColor"]);

        public static readonly BindableProperty IsShowRightTextProperty = BindableProperty.Create(propertyName: nameof(IsShowRightText),
                                                                                                  returnType: typeof(bool),
                                                                                                  declaringType: typeof(ThumListItem),
                                                                                                  defaultValue: false);

        public static readonly BindableProperty LongPressedProperty = BindableProperty.Create(propertyName: nameof(LongPressed),
                                                                                              returnType: typeof(ICommand),
                                                                                              declaringType: typeof(ThumListItem),
                                                                                              defaultValue: null);

        public static readonly BindableProperty RightIconSourceProperty = BindableProperty.Create(propertyName: nameof(RightIconSource),
                                                                                                  returnType: typeof(string),
                                                                                                  declaringType: typeof(ThumListItem),
                                                                                                  defaultValue: String.Empty);

        public static readonly BindableProperty RightIconTextColorProperty = BindableProperty.Create(propertyName: nameof(RightIconTextColor),
                                                                                                    returnType: typeof(Color),
                                                                                                    declaringType: typeof(ThumListItem),
                                                                                                    defaultValue: Color.FromHex("#dddddd"));

        public static readonly BindableProperty RightTextProperty = BindableProperty.Create(propertyName: nameof(RightText),
                                                                                            returnType: typeof(string),
                                                                                            declaringType: typeof(ThumListItem),
                                                                                            defaultValue: String.Empty);

        public static readonly BindableProperty SingleTapProperty = BindableProperty.Create(propertyName: nameof(SingleTap),
                                                                                            returnType: typeof(ICommand),
                                                                                            declaringType: typeof(ThumListItem),
                                                                                            defaultValue: null);

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(propertyName: nameof(Source),
                                                                                         returnType: typeof(string),
                                                                                         declaringType: typeof(ThumListItem),
                                                                                         defaultValue: String.Empty);

        public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: nameof(Text),
                                                                                       returnType: typeof(string),
                                                                                       declaringType: typeof(ThumListItem),
                                                                                       defaultValue: String.Empty);

        public string CommandParameter
        {
            get { return (string)GetValue(CommandParameterProperty); }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        public string Detail
        {
            get { return (string)GetValue(DetailProperty); }
            set
            {
                SetValue(DetailProperty, value);
            }
        }

        public Color GridBackgroundColor
        {
            get { return (Color)GetValue(GridBackgroundColorProperty); }
            set { SetValue(GridBackgroundColorProperty, value); }
        }

        public bool IsShowRightText
        {
            get { return (bool)GetValue(IsShowRightTextProperty); }
            set
            {
                SetValue(IsShowRightTextProperty, value);
            }
        }

        public ICommand LongPressed
        {
            get { return (ICommand)GetValue(LongPressedProperty); }
            set
            {
                SetValue(LongPressedProperty, value);
            }
        }

        public string RightIconSource
        {
            get { return (string)GetValue(RightIconSourceProperty); }
            set { SetValue(RightIconSourceProperty, value); }
        }

        public Color RightIconTextColor
        {
            get { return (Color)GetValue(RightIconTextColorProperty); }
            set { SetValue(RightIconTextColorProperty, value); }
        }

        public string RightText
        {
            get { return (string)GetValue(RightTextProperty); }
            set
            {
                SetValue(RightTextProperty, value);
            }
        }

        public ICommand SingleTap
        {
            get { return (ICommand)GetValue(SingleTapProperty); }
            set
            {
                SetValue(SingleTapProperty, value);
            }
        }

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion Properties

        #region Override

        protected override void OnBindingContextChanged()
        {
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

            base.OnBindingContextChanged();
        }

        #endregion Override
    }
}
