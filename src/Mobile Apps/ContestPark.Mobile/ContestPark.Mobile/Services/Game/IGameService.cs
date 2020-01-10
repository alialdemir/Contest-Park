using ContestPark.Mobile.Models.Duel;
using Prism.Navigation;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Game
{
    public interface IGameService
    {
        INavigationService NavigationService { get; set; }

        Task<bool> PushCategoryDetailViewAsync(short subCategoryId, bool isCategoryOpen, string subCategoryName);

        Task SubCategoriesDisplayActionSheetAsync(SelectedSubCategoryModel selectedSubCategory, bool isCategoryOpen);

        void SubCategoryShare(string Title);
    }
}
