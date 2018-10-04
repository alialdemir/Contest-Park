using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Category
{
    public interface ICategoryServices
    {
        Task<ServiceModel<CategoryModel>> CategoryListAsync(PagingModel pagingModel);

        Task<ServiceModel<SubCategorySearch>> CategorySearchAsync(short subCategoryId, PagingModel pagingModel);

        Task<bool> IsFollowUpStatusAsync(short subCategoryId);

        Task<int> FollowersCountAsync(short subCategoryId);

        Task<bool> SubCategoryFollowProgcess(short subCategoryId, bool isSubCategoryFollowUpStatus);

        Task<bool> FollowSubCategoryAsync(short subCategoryId);

        Task<bool> UnFollowSubCategoryAsync(short subCategoryId);

        Task<bool> OpenCategoryAsync(short subCategoryId);

        Task<ServiceModel<SubCategorySearch>> FollowingSubCategorySearchAsync(PagingModel pagingModel);
    }
}