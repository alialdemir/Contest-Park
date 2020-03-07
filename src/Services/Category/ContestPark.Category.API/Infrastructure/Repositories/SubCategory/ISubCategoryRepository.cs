using ContestPark.Category.API.Model;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;

namespace ContestPark.Category.API.Infrastructure.Repositories.SubCategory
{
    public interface ISubCategoryRepository
    {
        ServiceModel<CategoryModel> GetCategories(string userId, Languages language, PagingModel pagingModel);

        ServiceModel<SubCategoryModel> GetFollowedSubCategories(string userId, Languages language, PagingModel pagingModel);

        bool IsSubCategoryFree(short subCategoryId);

        bool IncreasingFollowersCount(short subCategoryId);

        bool DecreasingFollowersCount(short subCategoryId);

        SubCategoryDetailInfoModel GetSubCategoryDetail(short subCategoryId, Languages language, string userId);

        decimal GetSubCategoryPrice(short subCategoryId);
    }
}
