using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class CategorySearchViewModel : ViewModelBase<SubCategorySearch>
    {
        #region Private variable

        private Int16 _categoryId = 0;
        private readonly ICategoryServices _CategoryServices;
        private readonly IEventAggregator _eventAggregator;
        private readonly IGameService _gameService;

        #endregion Private variable

        #region Constructors

        public CategorySearchViewModel(ICategoryServices CategoryServices,
                                       IEventAggregator eventAggregator,
                                       IGameService gameService)
        {
            Title = ContestParkResources.CategorySearch;
            _CategoryServices = CategoryServices;
            _eventAggregator = eventAggregator;
            _gameService = gameService;
            EventSubscribe();
        }

        #endregion Constructors

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            //if (_CategoryId == -1) ServiceModel = await _followCategoryService.FollowingSubCategorySearchAsync(ServiceModel);// _CategoryId  -1 gelirse takip ettiğim kategoriler
            //  else
            if (_categoryId > 0)
            {
                ServiceModel = await _CategoryServices.CategorySearchAsync(_categoryId, ServiceModel);//0 gelirse tüm kategoriler demek 0 dan büyük ise ilgili kategoriyi getirir
                await base.InitializeAsync();
            }
            IsBusy = false;
        }

        /// <summary>
        /// Kategori giriş sayfasına git
        /// </summary>
        /// <param name="subCategoryId">Alt kategori ıd</param>
        /// <returns></returns>
        private void ExecutPushEnterPageCommandAsync(int subCategoryId)
        {
            SubCategorySearch selectedModel = Items.Where(p => p.SubCategoryId == subCategoryId).First();
            if (selectedModel == null)
                return;

            _gameService.PushCategoryDetailViewAsync(new SubCategoryModel()
            {
                PicturePath = selectedModel.PicturePath,
                SubCategoryId = selectedModel.SubCategoryId,
                SubCategoryName = selectedModel.SubCategoryName,
                DisplayPrice = selectedModel.DisplayPrice
            });
        }

        /// <summary>
        /// Event dinleme
        /// </summary>
        private void EventSubscribe()
        {
            _eventAggregator
                          .GetEvent<SubCategoryRefleshEvent>()
                          .Subscribe(() => RefleshCommand.Execute(null));
        }

        #endregion Methods

        #region Commands

        private ICommand pushEnterPageCommand;

        /// <summary>
        /// Yarışma kategorileri command
        /// </summary>
        public ICommand PushEnterPageCommand
        {
            get { return pushEnterPageCommand ?? (pushEnterPageCommand = new Command<int>((subCategoryId) => ExecutPushEnterPageCommandAsync(subCategoryId))); }
        }

        #endregion Commands

        #region Navigation

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("CategoryId")) _categoryId = parameters.GetValue<Int16>("CategoryId");

            base.OnNavigatingTo(parameters);
        }

        #endregion Navigation
    }
}