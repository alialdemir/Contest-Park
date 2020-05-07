using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components.ContestStore
{
    public partial class SpecialOfferView : ContentView
    {
        #region Constructor

        public SpecialOfferView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(propertyName: nameof(CommandParameter),
                                                                                                   returnType: typeof(string),
                                                                                                   declaringType: typeof(SpecialOfferView),
                                                                                                   defaultValue: string.Empty);

        public static readonly BindableProperty DetailProperty = BindableProperty.Create(propertyName: nameof(Detail),
                                                                                         returnType: typeof(string),
                                                                                         declaringType: typeof(SpecialOfferView),
                                                                                         defaultValue: String.Empty);

        public static readonly BindableProperty IsShowRightTextProperty = BindableProperty.Create(propertyName: nameof(IsShowRightText),
                                                                                                  returnType: typeof(bool),
                                                                                                  declaringType: typeof(SpecialOfferView),
                                                                                                  defaultValue: false);

        public static readonly BindableProperty LongPressedProperty = BindableProperty.Create(propertyName: nameof(LongPressed),
                                                                                              returnType: typeof(ICommand),
                                                                                              declaringType: typeof(SpecialOfferView),
                                                                                              defaultValue: null);

        public static readonly BindableProperty RightIconSourceProperty = BindableProperty.Create(propertyName: nameof(RightIconSource),
                                                                                                  returnType: typeof(string),
                                                                                                  declaringType: typeof(SpecialOfferView),
                                                                                                  defaultValue: String.Empty);

        public static readonly BindableProperty RightIconTextColorProperty = BindableProperty.Create(propertyName: nameof(RightIconTextColor),
                                                                                                    returnType: typeof(Color),
                                                                                                    declaringType: typeof(SpecialOfferView),
                                                                                                    defaultValue: Color.FromHex("#dddddd"));

        public static readonly BindableProperty RightTextProperty = BindableProperty.Create(propertyName: nameof(RightText),
                                                                                            returnType: typeof(string),
                                                                                            declaringType: typeof(SpecialOfferView),
                                                                                            defaultValue: String.Empty);

        public static readonly BindableProperty RightText2Property = BindableProperty.Create(propertyName: nameof(RightText2),
                                                                                            returnType: typeof(string),
                                                                                            declaringType: typeof(SpecialOfferView),
                                                                                            defaultValue: String.Empty);

        public static readonly BindableProperty RightText2TextColorProperty = BindableProperty.Create(propertyName: nameof(RightText2TextColor),
                                                                                                      returnType: typeof(Color),
                                                                                                      declaringType: typeof(SpecialOfferView),
                                                                                                      defaultValue: Color.Black);

        public static readonly BindableProperty RightText2TextDecorationsProperty = BindableProperty.Create(propertyName: nameof(RightText2TextDecorations),
                                                                                                      returnType: typeof(TextDecorations),
                                                                                                      declaringType: typeof(SpecialOfferView),
                                                                                                      defaultValue: TextDecorations.Strikethrough);

        public static readonly BindableProperty SingleTapProperty = BindableProperty.Create(propertyName: nameof(SingleTap),
                                                                                            returnType: typeof(ICommand),
                                                                                            declaringType: typeof(SpecialOfferView),
                                                                                            defaultValue: null);

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(propertyName: nameof(Source),
                                                                                         returnType: typeof(string),
                                                                                         declaringType: typeof(SpecialOfferView),
                                                                                         defaultValue: String.Empty);

        public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: nameof(Text),
                                                                                       returnType: typeof(string),
                                                                                       declaringType: typeof(SpecialOfferView),
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

        public string RightText2
        {
            get { return (string)GetValue(RightText2Property); }
            set
            {
                SetValue(RightText2Property, value);
            }
        }

        public TextDecorations RightText2TextDecorations
        {
            get { return (TextDecorations)GetValue(RightText2TextDecorationsProperty); }
            set
            {
                SetValue(RightText2TextDecorationsProperty, value);
            }
        }

        public Color RightText2TextColor
        {
            get { return (Color)GetValue(RightText2TextColorProperty); }
            set
            {
                SetValue(RightText2TextColorProperty, value);
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
    }
}
