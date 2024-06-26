﻿using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Models.User;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ContestPark.Mobile.ViewModels
{
    public class SignUpSelectSubCategoriesViewModel : ViewModelBase<CategoryModel>
    {
        #region Private variables

        private readonly ICategoryService _categoryService;
        private readonly ISettingsService _settingsService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IIdentityService _identityService;

        #endregion Private variables

        #region Constructor

        public SignUpSelectSubCategoriesViewModel(ICategoryService categoryService,
                                                  ISettingsService settingsService,
                                                  IAnalyticsService analyticsService,
                                                  INavigationService navigationService,
                                                  IPageDialogService pageDialogService,
                                                  IIdentityService identityService) : base(navigationService, pageDialogService)
        {
            _categoryService = categoryService;
            _settingsService = settingsService;
            _analyticsService = analyticsService;
            _identityService = identityService;
            ServiceModel.PageSize = 9999;// Şimdilik 9999 verdim kategorilerde safyalama yok
        }

        #endregion Constructor

        #region Properties

        public SignUpModel SignUp { get; set; } = new SignUpModel();

        private byte _selectedSubCategoryCount;

        public byte SelectedSubCategoryCount
        {
            get { return _selectedSubCategoryCount; }
            set
            {
                _selectedSubCategoryCount = value;
                RaisePropertyChanged(() => SelectedSubCategoryCount);
            }
        }

        #endregion Properties

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("SignUp"))
                SignUp = parameters.GetValue<SignUpModel>("SignUp");

            CategoryListCommand.Execute(null);

            base.Initialize(parameters);
        }

        /// <summary>
        /// Seçilen alt kategorinin kilidini açar
        /// </summary>
        /// <param name="selectedSubCategory">Seçilen alt kategori bilgisi</param>
        private async Task ExecuteClickSubCategoryCommand(SubCategoryModel selectedSubCategory)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            bool isExistsSubCategory = SignUp.SubCategories.Any(subCategoryId => selectedSubCategory.SubCategoryId == subCategoryId);
            if (!isExistsSubCategory && SelectedSubCategoryCount >= 3)
            {
                await DisplayAlertAsync(string.Empty,
                                        ContestParkResources.YouCanChooseaMaximumOfThreeCategories,
                                        ContestParkResources.Okay);

                IsBusy = false;

                return;
            }

            Items
                .FirstOrDefault(c => c.SubCategories.Any(sc => sc.SubCategoryId == selectedSubCategory.SubCategoryId))
                .SubCategories
                .FirstOrDefault(x => x.SubCategoryId == selectedSubCategory.SubCategoryId)
                .IsSubCategoryOpen = !selectedSubCategory.IsSubCategoryOpen;

            if (isExistsSubCategory)
            {
                SignUp.SubCategories.Remove(selectedSubCategory.SubCategoryId);
            }
            else
            {
                SignUp.SubCategories.Add(selectedSubCategory.SubCategoryId);
            }

            SelectedSubCategoryCount = (byte)SignUp.SubCategories.Count();

            IsBusy = false;
        }

        /// <summary>
        /// Üye olma isteği atıp sonra login olur
        /// </summary>
        private async Task ExecuteSignUpCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (SelectedSubCategoryCount < 3)
            {
                await DisplayAlertAsync(string.Empty,
                                        ContestParkResources.YouMustChooseAtLeastThreeCategories,
                                        ContestParkResources.Okay);

                IsBusy = false;

                return;
            }

            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            bool isSuccess = await _identityService.SignUpAsync(SignUp);
            if (isSuccess)
            {
                await LoginProcessAsync();
            }

            UserDialogs.Instance.HideLoading();

            IsBusy = false;
        }

        /// <summary>
        /// Üye olduktan sonra login olmakk için
        /// </summary>
        private async Task LoginProcessAsync()
        {
            UserToken token = await _identityService.GetTokenAsync(new LoginModel
            {
                UserName = SignUp.UserName,
                Password = SignUp.Password
            });

            bool isTokenExits = token != null;

            _analyticsService.SendEvent("Login", "Üye ol", isTokenExits ? "Success" : "Fail");

            if (isTokenExits)
            {
                _settingsService.SetTokenInfo(token);

                _categoryService.RemoveCategoryListCache(new PagingModel { PageSize = 9999 });// Kategori sayfasında tekrar yeniden kategorileri çekmesi için cache temizledim

                UserInfoModel currentUser = await _identityService.GetUserInfo();
                if (currentUser != null)
                {
                    _settingsService.RefreshCurrentUser(currentUser);
                    _analyticsService.SetUserId(currentUser.UserId);
                }

                await NavigateToInitialized<AppShell>();
            }
            else
            {
                await DisplayAlertAsync("",
                                        ContestParkResources.MembershipWasSuccessfulButTheLoginFailedPleaseLoginFromTheLoginPage,
                                        ContestParkResources.Okay);
            }
        }

        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            _categoryService.RemoveCategoryListCache(new PagingModel { PageSize = 9999 });// Kategori sayfasında tekrar yeniden kategorileri çekmesi için cache temizledim

            return base.GoBackAsync(parameters, useModalNavigation);
        }

        /// <summary>
        /// Kategori listesi getirir
        /// </summary>
        private async Task ExecuteCategoryListCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = await _categoryService.CategoryListAsync(ServiceModel, IsRefreshing);

            if (Items != null && Items.Any())
            {
                Items
               .ToList()
               .ForEach(c => c.SubCategories?.ForEach(sc => sc.IsSubCategoryOpen = false));
            }

            SelectedSubCategoryCount = 0;

            SignUp.SubCategories = new List<short>();

            IsBusy = false;
        }

        #endregion Methods

        #region Command

        private ICommand CategoryListCommand => new CommandAsync(ExecuteCategoryListCommandAsync);

        private ICommand _clickSubCategoryCommand;

        public ICommand ClickSubCategoryCommand => _clickSubCategoryCommand ?? (_clickSubCategoryCommand = new CommandAsync<SubCategoryModel>(ExecuteClickSubCategoryCommand));
        public ICommand SignUpCommand => new CommandAsync(ExecuteSignUpCommand);

        #endregion Command
    }
}
