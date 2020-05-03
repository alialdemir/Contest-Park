using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Ranking;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    
    public partial class RankTopView : ContentView
    {
        #region Constructor

        public RankTopView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty BalanceTypeProperty = BindableProperty.Create(propertyName: nameof(BalanceType),
                                                                                                  returnType: typeof(BalanceTypes),
                                                                                                  declaringType: typeof(RankTopView),
                                                                                                  defaultValue: null);

        public BalanceTypes BalanceType
        {
            get { return (BalanceTypes)GetValue(BalanceTypeProperty); }
            set { SetValue(BalanceTypeProperty, value); }
        }

        public static readonly BindableProperty GotoProfilePageCommandProperty = BindableProperty.Create(propertyName: nameof(GotoProfilePageCommand),
                                                                                         returnType: typeof(ICommand),
                                                                                         declaringType: typeof(RankTopView),
                                                                                         defaultValue: null);

        public ICommand GotoProfilePageCommand
        {
            get { return (ICommand)GetValue(GotoProfilePageCommandProperty); }
            set
            {
                SetValue(GotoProfilePageCommandProperty, value);
            }
        }

        public static readonly BindableProperty RankProperty = BindableProperty.Create(propertyName: nameof(Rank),
                                                                                        returnType: typeof(Ranks),
                                                                                        declaringType: typeof(RankTopView),
                                                                                        defaultValue: null);

        public Ranks Rank
        {
            get { return (Ranks)GetValue(RankProperty); }
            set
            {
                SetValue(RankProperty, value);
            }
        }

        public static readonly BindableProperty RankGifProperty = BindableProperty.Create(propertyName: nameof(RankGif),
                                                                                        returnType: typeof(Xamarin.Forms.ImageSource),
                                                                                        declaringType: typeof(RankTopView),
                                                                                        defaultValue: null);

        public Xamarin.Forms.ImageSource RankGif
        {
            get { return (Xamarin.Forms.ImageSource)GetValue(RankGifProperty); }
            set
            {
                SetValue(RankGifProperty, value);
            }
        }

        public static readonly BindableProperty RankingProperty = BindableProperty.Create(propertyName: nameof(Ranking),
                                                                                        returnType: typeof(RankingModel),
                                                                                        declaringType: typeof(RankTopView),
                                                                                        defaultValue: null);

        public RankingModel Ranking
        {
            get { return (RankingModel)GetValue(RankingProperty); }
            set
            {
                SetValue(RankingProperty, value);
            }
        }

        #endregion Properties

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            switch (Rank)
            {
                case Ranks.First:
                    RankGif = "rank1.gif".ToResourceImage();
                    profilePicture.BorderColor = "#FEB833";
                    profilePicture.HeightRequest = profilePicture.WidthRequest = 100;
                    break;

                case Ranks.Secound:
                    RankGif = "rank2.gif".ToResourceImage();
                    profilePicture.BorderColor = "#ACACAC";
                    profilePicture.HeightRequest = profilePicture.WidthRequest = 70;
                    break;

                case Ranks.Third:
                    RankGif = "rank3.gif".ToResourceImage();
                    profilePicture.BorderColor = "#C38651";
                    profilePicture.HeightRequest = profilePicture.WidthRequest = 70;
                    break;
            }

            switch (BalanceType)
            {
                case BalanceTypes.Money:
                    imgCoins.Source = "doublecoinstl.png".ToResourceImage();
                    Grid.SetColumn(lblScore, 0);
                    break;

                case BalanceTypes.Gold:
                    imgCoins.Source = "doublecoins.png".ToResourceImage();
                    Grid.SetColumn(lblScore, 0);
                    break;

                default:
                    Grid.SetColumnSpan(lblScore, 2);
                    imgCoins.IsVisible = false;
                    break;
            }
        }

        #endregion Override
    }
}
