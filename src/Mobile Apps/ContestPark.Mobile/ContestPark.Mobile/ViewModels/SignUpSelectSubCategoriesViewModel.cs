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
using System.Threading.Tasks;

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
                                                  IIdentityService identityService,
                                                  INavigationService navigationService) : base(navigationService: navigationService)
        {
            _categoryService = categoryService;
            _settingsService = settingsService;
            _analyticsService = analyticsService;
            _identityService = identityService;
            ServiceModel.PageSize = 9999;// Şimdilik 9999 verdim kategorilerde safyalama yok
        }

        #endregion Constructor

        public SignUpModel SignUp { get; set; }

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

        private async Task SelectSubCategories()
        {
            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            bool isSuccess = await _identityService.SignUpAsync(SignUp);

            if (isSuccess)
            {
                await LoginProcessAsync();
            }
            UserDialogs.Instance.HideLoading();
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
    }
}
