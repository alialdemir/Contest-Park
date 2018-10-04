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

        public static readonly BindableProperty RightIconSourceProperty = BindableProperty.Create(propertyName: nameof(RightIconSource),
                                                                                                    returnType: typeof(string),
                                                                                                    declaringType: typeof(ListItemView),
                                                                                                    defaultValue: String.Empty);

        public string RightIconSource
        {
            get { return (string)GetValue(RightIconSourceProperty); }
            set { SetValue(RightIconSourceProperty, value); }
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

        public static readonly BindableProperty RightTextProperty = BindableProperty.Create(propertyName: nameof(RightText),
                                                                                                    returnType: typeof(string),
                                                                                                    declaringType: typeof(ListItemView),
                                                                                                    defaultValue: String.Empty);

        public string RightText
        {
            get { return (string)GetValue(RightTextProperty); }
            set
            {
                SetValue(RightTextProperty, value);
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

        public static readonly BindableProperty IsShowRightTextProperty = BindableProperty.Create(propertyName: nameof(IsShowRightText),
                                                                                                    returnType: typeof(bool),
                                                                                                    declaringType: typeof(ListItemView),
                                                                                                    defaultValue: false);

        public bool IsShowRightText
        {
            get { return (bool)GetValue(IsShowRightTextProperty); }
            set
            {
                SetValue(IsShowRightTextProperty, value);
            }
        }

        #endregion Properties

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            cstmGrid.LongPressed = LongPressed;
            cstmGrid.SingleTap = SingleTap;
            cstmGrid.CommandParameter = CommandParameter;
        }

        #endregion Override
    }
}