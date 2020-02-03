using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Ranking;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RankTopView : ContentView
    {
        #region Constructor

        public RankTopView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

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
                                                                                        returnType: typeof(string),
                                                                                        declaringType: typeof(RankTopView),
                                                                                        defaultValue: string.Empty);

        public string RankGif
        {
            get { return GetValue(RankGifProperty).ToString(); }
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
                    RankGif = "rank1.gif";
                    profilePicture.BorderColor = "#FEB833";
                    profilePicture.HeightRequest = profilePicture.WidthRequest = 100;
                    break;

                case Ranks.Secound:
                    RankGif = "rank2.gif";
                    profilePicture.BorderColor = "#ACACAC";
                    profilePicture.HeightRequest = profilePicture.WidthRequest = 70;
                    break;

                case Ranks.Third:
                    RankGif = "rank3.gif";
                    profilePicture.BorderColor = "#C38651";
                    profilePicture.HeightRequest = profilePicture.WidthRequest = 70;
                    break;
            }
        }

        #endregion Override
    }
}
