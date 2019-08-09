using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Services.Game;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubCategoryCard : CustomGrid
    {
        #region Constructor

        public SubCategoryCard()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        private string _subCategoryImageSource;
        private string _subCategoryNam;

        public string DisplayPrice
        {
            set
            {
                lblDisplayPrice.Text = value;
            }
        }

        public bool IsBusy { get; set; }
        public bool IsCategoryOpen { get; set; }

        public short SubCategoryId { get; set; }

        public string SubCategoryImageSource
        {
            get => _subCategoryImageSource;
            set
            {
                _subCategoryImageSource = value;

                if (value.StartsWith("http://") || value.StartsWith("https://"))
                {
                    imgSubCategoryImageSource.Source = Xamarin.Forms.ImageSource.FromUri(new Uri(value));
                }
                else
                {
                    imgSubCategoryImageSource.Source = Xamarin.Forms.ImageSource.FromFile(value);
                }
            }
        }

        public string SubCategoryName
        {
            get => _subCategoryNam;
            set
            {
                _subCategoryNam = value;
                lblSubCategoryName.Text = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Alt kategoriye uzun basınca ActionSheet gösterir
        /// </summary>
        private void AddLongPressed()
        {
            customGrid.LongPressed = new Command(async () =>
            {
                if (IsBusy)
                    return;

                IsBusy = true;

                IGameService gameService = RegisterTypesConfig.Container.Resolve<IGameService>();

                await gameService?.SubCategoriesDisplayActionSheetAsync(SubCategoryId, SubCategoryName, IsCategoryOpen);

                IsBusy = false;
            });
        }

        /// <summary>
        /// Alt kategoriye tıklanınca kategori detaya gider
        /// </summary>
        private void AddSingleTap()
        {
            customGrid.SingleTap = new Command(async () =>
            {
                if (IsBusy)
                    return;

                IsBusy = true;

                IGameService gameService = RegisterTypesConfig.Container.Resolve<IGameService>();

                await gameService?.PushCategoryDetailViewAsync(SubCategoryId, IsCategoryOpen);

                IsBusy = false;
            });
        }

        #endregion Methods

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            AddLongPressed();

            AddSingleTap();

            imgSubCategoryImageSource.Transformations = new List<ITransformation>() { new CircleTransformation() };

            goldBadge.IsVisible = !IsCategoryOpen;
        }

        #endregion Override
    }
}
