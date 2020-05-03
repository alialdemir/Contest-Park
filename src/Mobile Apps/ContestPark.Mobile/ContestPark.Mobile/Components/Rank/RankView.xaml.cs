using ContestPark.Mobile.Enums;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RankView : ContentView
    {
        #region Constructors

        public RankView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Property

        public static readonly BindableProperty BalanceTypeProperty = BindableProperty.Create(propertyName: nameof(BalanceType),
                                                                                                    returnType: typeof(BalanceTypes),
                                                                                                    declaringType: typeof(RankView),
                                                                                                    defaultValue: null);

        public BalanceTypes BalanceType
        {
            get { return (BalanceTypes)GetValue(BalanceTypeProperty); }
            set { SetValue(BalanceTypeProperty, value); }
        }

        public static readonly BindableProperty GotoProfilePageCommandProperty = BindableProperty.Create(propertyName: nameof(GotoProfilePageCommand),
                                                                                           returnType: typeof(ICommand),
                                                                                           declaringType: typeof(RankView),
                                                                                           defaultValue: null);

        public static readonly BindableProperty RankProperty = BindableProperty.Create(propertyName: nameof(Rank),
                                                                                                        returnType: typeof(string),
                                                                                                declaringType: typeof(RankView),
                                                                                                defaultValue: String.Empty);

        public ICommand GotoProfilePageCommand
        {
            get { return (ICommand)GetValue(GotoProfilePageCommandProperty); }
            set
            {
                SetValue(GotoProfilePageCommandProperty, value);
            }
        }

        public string Rank
        {
            get { return (string)GetValue(RankProperty); }
            set { SetValue(RankProperty, value); }
        }

        #endregion Property

        #region Overrides

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            switch (BalanceType)
            {
                case BalanceTypes.Money:
                    imgCoins.Source = "doublecoinstl.png".ToResourceImage();
                    break;

                case BalanceTypes.Gold:
                    imgCoins.Source = "doublecoins.png".ToResourceImage();
                    break;

                default:

                    lblScore.Margin = new Thickness(0, 0, 16, 0);
                    imgCoins.IsVisible = false;
                    break;
            }
        }

        #endregion Overrides
    }
}
