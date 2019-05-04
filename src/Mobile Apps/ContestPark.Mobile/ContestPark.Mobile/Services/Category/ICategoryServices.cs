using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Category
{
    public interface ICategoryService
    {
        Task<ServiceModel<CategoryModel>> CategoryListAsync(PagingModel pagingModel);

        Task<Models.Categories.CategoryDetail.CategoryDetailModel> GetSubCategoryDetail(short subCategoryId);

        Task<bool> OpenCategoryAsync(short subCategoryId);

        Task<ServiceModel<SearchModel>> SearchAsync(short subCategoryId, PagingModel pagingModel);

        Task<ServiceModel<SearchModel>> SearchAsync(string searchText, short subCategoryId, PagingModel pagingModel);
    }
}