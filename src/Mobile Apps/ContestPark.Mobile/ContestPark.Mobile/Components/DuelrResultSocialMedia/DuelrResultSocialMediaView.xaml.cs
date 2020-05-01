using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Duel.DuelResultSocialMedia;
using FFImageLoading.Transformations;
using System;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components.DuelResultSocialMedia
{
    /// <summary>
    /// Sadece sosyal medyada resim paylaşmaK için yazıldı her hangi biryerde kullanmayınız
    /// </summary>
    public partial class DuelResultSocialMediaView : ContentView
    {
        #region Constructor

        public DuelResultSocialMediaView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Dinamik resim oluşturulduğu için componentlere buradan set ettik
        /// </summary>
        public DuelResultSocialMediaModel ViewModel
        {
            set
            {
                DuelResultSocialMediaModel viewModel = value;

                if (viewModel != null)
                {
                    FounderImage.Source = FileImageSource.FromUri(new Uri(viewModel.FounderProfilePicturePath));
                    OpponentImage.Source = FileImageSource.FromUri(new Uri(viewModel.OpponentProfilePicturePath));

                    if (FounderImage.Transformations.Count == 1) FounderImage.Transformations.RemoveAt(0);
                    if (OpponentImage.Transformations.Count == 1) OpponentImage.Transformations.RemoveAt(0);

                    FounderImage.Transformations.Add(new CircleTransformation(20, viewModel.FounderColor));
                    OpponentImage.Transformations.Add(new CircleTransformation(20, viewModel.OpponentColor));

                    //Gold.Text = viewModel.Gold.ToString("##,#").Replace(",", ".");
                    doubleCoins.BalanceType = viewModel.BalanceType;
                    doubleCoins.DisplayCoins = viewModel.Gold.ToString("##,#").Replace(",", ".");

                    WinnerOrLoseText2.Text = ContestParkResources.Was;
                    WinnerOrLoseText3.Text = ContestParkResources.Victorious;

                    if (viewModel.FounderScore > viewModel.OpponentScore)
                    {
                        WinnerOrLoseText1.Text = viewModel.FounderFullName;
                    }
                    else if (viewModel.OpponentScore > viewModel.FounderScore)
                    {
                        WinnerOrLoseText1.Text = viewModel.OpponentFullName;
                    }
                    else
                    {
                        WinnerOrLoseText3.Text = ContestParkResources.Tie + "!";
                        WinnerOrLoseText3.TextColor = (Color)Application.Current.Resources["Primary"];
                        WinnerOrLoseText2.IsVisible = false;
                        WinnerOrLoseText1.IsVisible = false;
                    }

                    OpponentScore.Text = viewModel.OpponentScore.ToString();
                    FounderScore.Text = viewModel.FounderScore.ToString();

                    FounderFullName.Text = viewModel.FounderFullName;
                    FounderFullName.TextColor = Color.FromHex(viewModel.FounderColor);

                    OpponentFullName.Text = viewModel.OpponentFullName;
                    OpponentFullName.TextColor = Color.FromHex(viewModel.OpponentColor);

                    SubCategoryName.Text = viewModel.SubCategoryName;

                    Date.Text = viewModel.Date;

                    SubCategoryPicturePath.Source = FileImageSource.FromUri(new Uri(viewModel.SubCategoryPicturePath));
                }
            }
        }

        #endregion Properties
    }
}
