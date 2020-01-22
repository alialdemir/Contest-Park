using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Services.Game;
using Prism.Ioc;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubCategoryCard : ContentView
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

        public string DisplayPrice { get; set; }
        public bool IsBusy { get; set; }

        public static readonly BindableProperty IsCategoryOpenProperty = BindableProperty.Create(propertyName: nameof(IsCategoryOpen),
                                                                                                 returnType: typeof(bool),
                                                                                                 declaringType: typeof(SubCategoryCard),
                                                                                                 defaultValue: false);

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
                    imgSubCategoryImageSource.Source = ImageSource.FromUri(new Uri(value));
                }
                else
                {
                    imgSubCategoryImageSource.Source = ImageSource.FromFile(value);
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

        private ICommand _subCategoriesDisplayActionSheetCommand;

        public static readonly BindableProperty SubCategoriesDisplayActionSheetCommandProperty = BindableProperty.Create(propertyName: nameof(SubCategoriesDisplayActionSheetCommand),
                                                                                                                         returnType: typeof(ICommand),
                                                                                                                         declaringType: typeof(SubCategoryCard),
                                                                                                                         defaultValue: null);

        /// <summary>
        /// Alt kategori display alert command
        /// </summary>
        public ICommand SubCategoriesDisplayActionSheetCommand
        {
            get
            {
                return _subCategoriesDisplayActionSheetCommand ?? (_subCategoriesDisplayActionSheetCommand = new Command(async () => await AddLongPressed()));
            }
        }

        private ICommand _pushCategoryDetailViewCommand;

        public static readonly BindableProperty PushCategoryDetailViewCommandProperty = BindableProperty.Create(propertyName: nameof(PushCategoryDetailViewCommand),
                                                                                                                returnType: typeof(ICommand),
                                                                                                                declaringType: typeof(SubCategoryCard),
                                                                                                                defaultValue: null);

        public ICommand PushCategoryDetailViewCommand
        {
            get
            {
                return _pushCategoryDetailViewCommand ?? (_pushCategoryDetailViewCommand = new Command(async () => await AddSingleTap()));
            }
        }

        private IGameService _gameService;

        public IGameService GameService
        {
            get
            {
                if (_gameService == null)
                    _gameService = RegisterTypesConfig.Container.Resolve<IGameService>();

                return _gameService;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Alt kategoriye uzun basınca ActionSheet gösterir
        /// </summary>
        private async Task AddLongPressed()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await GameService?.SubCategoriesDisplayActionSheetAsync(new SelectedSubCategoryModel
            {
                SubcategoryId = SubCategoryId,
                SubcategoryName = SubCategoryName,
                SubCategoryPicturePath = SubCategoryImageSource
            }, IsCategoryOpen);

            IsBusy = false;
        }

        /// <summary>
        /// Alt kategoriye tıklanınca kategori detaya gider
        /// </summary>
        private async Task AddSingleTap()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await GameService?.PushCategoryDetailViewAsync(SubCategoryId,
                                                           IsCategoryOpen,
                                                           SubCategoryName);

            IsBusy = false;
        }

        #endregion Methods

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            lblDisplayPrice.IsVisible = goldBadge.IsVisible = !IsCategoryOpen;
        }

        #endregion Override
    }
}
