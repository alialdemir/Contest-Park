using ContestPark.Mobile.Enums;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DobleCoinsView : ContentView
    {
        #region Constructor

        public DobleCoinsView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty DisplayCoinsProperty = BindableProperty.Create(propertyName: nameof(DisplayCoins),
                                                                                                returnType: typeof(string),
                                                                                                declaringType: typeof(DobleCoinsView),
                                                                                                defaultValue: string.Empty);

        public string DisplayCoins
        {
            get { return (string)GetValue(DisplayCoinsProperty); }
            set { SetValue(DisplayCoinsProperty, value); }
        }

        public static readonly BindableProperty BalanceTypeProperty = BindableProperty.Create(propertyName: nameof(BalanceType),
                                                                                              returnType: typeof(BalanceTypes),
                                                                                              declaringType: typeof(DobleCoinsView),
                                                                                              defaultValue: BalanceTypes.Gold);

        public BalanceTypes BalanceType
        {
            get { return (BalanceTypes)GetValue(BalanceTypeProperty); }
            set { SetValue(BalanceTypeProperty, value); }
        }

        public static readonly BindableProperty CoinsProperty = BindableProperty.Create(propertyName: nameof(Coins),
                                                                                        returnType: typeof(Enums.Coins),
                                                                                        declaringType: typeof(DobleCoinsView),
                                                                                        defaultValue: Enums.Coins.Positive);

        public Enums.Coins Coins
        {
            get { return (Enums.Coins)GetValue(CoinsProperty); }
            set { SetValue(CoinsProperty, value); }
        }

        public static readonly BindableProperty CoinSizeProperty = BindableProperty.Create(propertyName: nameof(CoinSize),
                                                                                           returnType: typeof(Enums.CoinSize),
                                                                                           declaringType: typeof(DobleCoinsView),
                                                                                           defaultValue: Enums.CoinSize.Large);

        public Enums.CoinSize CoinSize
        {
            get { return (Enums.CoinSize)GetValue(CoinSizeProperty); }
            set { SetValue(CoinSizeProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(propertyName: nameof(TextColor),
                                                                                            returnType: typeof(Color),
                                                                                            declaringType: typeof(DobleCoinsView),
                                                                                            defaultValue: Color.FromHex("#8EF0A7"));

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        #endregion Properties

        #region Overrides

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            switch (CoinSize)
            {
                case CoinSize.Small:
                    imgCoins.WidthRequest = 32;
                    imgCoins.HeightRequest = 32;
                    break;

                default:
                    imgCoins.WidthRequest = 64;
                    imgCoins.HeightRequest = 64;
                    break;
            }
        }

        #endregion Overrides
    }
}
