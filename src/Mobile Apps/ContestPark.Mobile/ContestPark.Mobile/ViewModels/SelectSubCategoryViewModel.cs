﻿using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Rg.Plugins.Popup.Contracts;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class SelectSubCategoryViewModel : ViewModelBase<CategoryModel>
    {
        #region Private variables

        private readonly ICategoryService _categoryService;
        private readonly IGameService _gameService;

        #endregion Private variables

        #region Constructor

        public SelectSubCategoryViewModel(ICategoryService categoryService, IGameService gameService, IPopupNavigation popupNavigation) : base(popupNavigation: popupNavigation)
        {
            _categoryService = categoryService;
            _gameService = gameService;
            ServiceModel.PageSize = 9999;// Şimdilik 9999 verdim kategorilerde safyalama yok
        }

        #endregion Constructor

        #region Properties

        public string OpponentUserId { get; set; }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = await _categoryService.CategoryListAsync(ServiceModel);

            await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Düello bahis panelini aç
        /// </summary>
        private void ExecuteduelOpenPanelCommandAsync(SubCategoryModel subCategory)
        {
            if (IsBusy || subCategory == null)
                return;

            IsBusy = true;

            ClosePopupCommand.Execute(null);

            if (subCategory.IsCategoryOpen)
            {
                PushPopupPageAsync(new DuelBettingPopupView()
                {
                    SelectedSubCategory = new Models.Duel.SelectedSubCategoryModel
                    {
                        SubcategoryId = subCategory.SubCategoryId,
                        SubcategoryName = subCategory.SubCategoryName,
                        SubCategoryPicturePath = subCategory.PicturePath,
                    },
                    OpponentUserId = OpponentUserId,
                });
            }
            else
            {
                _gameService.OpenSubCategoryProgcess(subCategory.SubCategoryId);
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand _duelOpenPanelCommand;

        /// <summary>
        /// Düello bahis paneli aç command
        /// </summary>
        public ICommand DuelOpenPanelCommand
        {
            get { return _duelOpenPanelCommand ?? (_duelOpenPanelCommand = new Command<SubCategoryModel>((subCategory) => ExecuteduelOpenPanelCommandAsync(subCategory))); }
        }

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }

        #endregion Commands
    }
}