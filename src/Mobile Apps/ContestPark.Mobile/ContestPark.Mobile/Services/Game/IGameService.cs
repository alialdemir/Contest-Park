using Prism.Navigation;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Game
{
    public interface IGameService
    {
        Task<bool> PushCategoryDetailViewAsync(short subCategoryId, string subCategoryName, string subCategoryPicturePath, bool isCategoryOpen);

        INavigationService NavigationService { get; set; }

        void SubCategoryShare(string Title);

        Task SubCategoriesDisplayActionSheetAsync(short subCategoryId, string subCategoryName, bool isCategoryOpen, string subCategoryPicturePath);
    }
}