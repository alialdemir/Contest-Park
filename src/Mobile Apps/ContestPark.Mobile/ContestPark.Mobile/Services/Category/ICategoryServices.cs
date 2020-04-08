using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Categories.CategoryDetail;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.RequestProvider;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Category
{
    public interface ICategoryService
    {
        Task<ServiceModel<SearchModel>> FollowedSubCategoriesAsync(string searchText, PagingModel pagingModel);

        Task<bool> FollowSubCategoryAsync(short subCategoryId);

        Task<bool> IsFollowUpStatusAsync(short subCategoryId);

        Task<bool> SubCategoryFollowProgcess(short subCategoryId, bool isSubCategoryFollowUpStatus);

        Task<bool> UnFollowSubCategoryAsync(short subCategoryId);

        Task<ServiceModel<CategoryModel>> CategoryListAsync(PagingModel pagingModel, bool isAllOpen = false);

        Task<CategoryDetailModel> GetSubCategoryDetail(short subCategoryId);

        Task<ResponseModel<string>> OpenCategoryAsync(short subCategoryId, BalanceTypes balanceType = BalanceTypes.Gold);

        Task<ServiceModel<SearchModel>> SearchAsync(string searchText, short categoryId, PagingModel pagingModel);
    }
}
