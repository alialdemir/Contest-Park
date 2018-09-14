using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class CategoryDetailViewModel : ViewModelBase//<PostListModel>
    {
        #region Private variable

        private Int16 _subCategoryId = 0;

        #endregion Private variable

        #region Constructor

        public CategoryDetailViewModel(INavigationService navigationService,
                                       IPopupNavigation popupNavigation) : base(navigationService, popupNavigation: popupNavigation)
        {
        }

        #endregion Constructor

        #region Properties

        private string _subCategoryPicturePath;

        public string SubCategoryPicturePath
        {
            get
            {
                return _subCategoryPicturePath;
            }
            set
            {
                _subCategoryPicturePath = value;
                RaisePropertyChanged(() => SubCategoryPicturePath);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Düello bahis panelini aç
        /// </summary>
        private async Task ExecuteduelOpenPanelCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await PushPopupPageAsync(new DuelBettingPopupView()
            {
                SubcategoryId = _subCategoryId,
                SubcategoryName = Title,
                SubCategoryPicturePath = SubCategoryPicturePath,
            });

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand duelOpenPanelCommand;

        /// <summary>
        /// Düello bahis paneli aç command
        /// </summary>
        public ICommand DuelOpenPanelCommand
        {
            get { return duelOpenPanelCommand ?? (duelOpenPanelCommand = new Command(async () => await ExecuteduelOpenPanelCommandAsync())); }
        }

        #endregion Commands

        #region Navigation

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SubCategoryName")) Title = parameters.GetValue<string>("SubCategoryName");
            if (parameters.ContainsKey("SubCategoryPicturePath")) SubCategoryPicturePath = parameters.GetValue<string>("SubCategoryPicturePath");
            if (parameters.ContainsKey("SubCategoryId")) _subCategoryId = parameters.GetValue<Int16>("SubCategoryId");

            base.OnNavigatingTo(parameters);
        }

        #endregion Navigation
    }
}