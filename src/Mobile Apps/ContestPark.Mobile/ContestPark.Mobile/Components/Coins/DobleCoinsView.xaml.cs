
using ContestPark.Mobile.Enums;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components.Coins
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DobleCoinsView : ContentView
    {

        #region Constructor

        public DobleCoinsView()
        {
            InitializeComponent();
        }

        #endregion

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
        #endregion

        #region Overrides
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            switch (BalanceType)
            {
                case BalanceTypes.Money:
                    imgCoins.Source = ImageSource.FromFile("doublecoinstl.png");
                    break;
                default:
                    imgCoins.Source = ImageSource.FromFile("doublecoins.png");
                    break;
            }

            switch (CoinSize)
            {
                case CoinSize.Small:
                    imgCoins.HeightRequest = 80;
                    imgCoins.WidthRequest = 80;
                    lblCoins.FontSize = 16;
                    break;
                default:
                    imgCoins.HeightRequest = 100;
                    imgCoins.WidthRequest = 100;
                    lblCoins.FontSize = 22;
                    break;
            }

            lblCoins.Text = (Coins == Enums.Coins.Negative ? "-" : "+") + DisplayCoins;
            lblCoins.TextColor = Color.FromHex(Coins == Enums.Coins.Negative ? "#FB1A1A" : "#8EF0A7");
        }
        #endregion
    }
}