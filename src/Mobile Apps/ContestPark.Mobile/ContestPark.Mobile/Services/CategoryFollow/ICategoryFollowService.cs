using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.CategoryFollow
{
    public interface ICategoryFollowService
    {
        Task<int> FollowersCountAsync(short subCategoryId);

        Task<ServiceModel<SearchModel>> FollowingSubCategorySearchAsync(string searchText, short subCategoryId, PagingModel pagingModel);

        Task<bool> FollowSubCategoryAsync(short subCategoryId);

        Task<bool> IsFollowUpStatusAsync(short subCategoryId);

        Task<bool> SubCategoryFollowProgcess(short subCategoryId, bool isSubCategoryFollowUpStatus);

        Task<bool> UnFollowSubCategoryAsync(short subCategoryId);
    }
}