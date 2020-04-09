using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Login;
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
using Xamarin.Forms;

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
                                                  IPageDialogService pageDialogService,
                                                  IIdentityService identityService,
                                                  INavigationService navigationService) : base(navigationService, pageDialogService)
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

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = await _categoryService.CategoryListAsync(ServiceModel);

            SelectedSubCategoryCount = 0;

            SignUp.SubCategories = new List<short>();

            await base.InitializeAsync();

            IsBusy = false;
        }

        private async Task ExecuteClickSubCategoryCommand(SubCategoryModel selectedSubCategory)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (!SignUp.SubCategories.Any(subCategoryId => selectedSubCategory.SubCategoryId == subCategoryId) && SelectedSubCategoryCount >= 3)
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
                .IsCategoryOpen = !selectedSubCategory.IsCategoryOpen;

            if (selectedSubCategory.IsCategoryOpen)
            {
                SignUp.SubCategories.Add(selectedSubCategory.SubCategoryId);

                SelectedSubCategoryCount += 1;
            }
            else
            {
                SignUp.SubCategories.Remove(selectedSubCategory.SubCategoryId);

                SelectedSubCategoryCount -= 1;
            }

            IsBusy = false;
        }

        /// <summary>
        /// Üye olma isteği atıp sonra login olur
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteSignUpCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            if (SelectedSubCategoryCount < 3)
            {
                await DisplayAlertAsync(string.Empty,
                                        ContestParkResources.YouCanChooseaMaximumOfThreeCategories,
                                        ContestParkResources.Okay);

                IsBusy = false;

                return;
            }

            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            //bool isSuccess = await _identityService.SignUpAsync(SignUp);

            //if (isSuccess)
            //{
            //    await LoginProcessAsync();
            //}
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

                UserInfoModel currentUser = await _identityService.GetUserInfo();
                if (currentUser != null)
                {
                    _settingsService.RefreshCurrentUser(currentUser);
                }

                _settingsService.SignUpCount += 1;

                _analyticsService.SetUserId(_settingsService.CurrentUser.UserId);

                await PushNavigationPageAsync($"app:///{nameof(AppShell)}?appModuleRefresh=OnInitialized");
            }
            else
            {
                await DisplayAlertAsync("",
                                        ContestParkResources.MembershipWasSuccessfulButTheLoginFailedPleaseLoginFromTheLoginPage,
                                        ContestParkResources.Okay);
            }
        }

        #endregion Methods

        #region Command

        private ICommand _clickSubCategoryCommand;

        public ICommand ClickSubCategoryCommand => _clickSubCategoryCommand ?? (_clickSubCategoryCommand = new Command<SubCategoryModel>(async (selectedSubCategory) => await ExecuteClickSubCategoryCommand(selectedSubCategory)));
        public ICommand SignUpCommand => new Command(async () => await ExecuteSignUpCommand());

        #endregion Command
    }
}
