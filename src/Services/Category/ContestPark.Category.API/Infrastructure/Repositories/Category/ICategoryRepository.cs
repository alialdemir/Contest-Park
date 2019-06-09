using ContestPark.Category.API.Model;
using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.Enums;

namespace ContestPark.Category.API.Infrastructure.Repositories.Category
{
    public interface ICategoryRepository
    {
        ServiceModel<CategoryModel> GetCategories(string userId, Languages language, PagingModel pagingModel);

        ServiceModel<SubCategoryModel> GetFollowedSubCategories(string userId, Languages language, PagingModel pagingModel);

        bool IsSubCategoryFree(string subCategoryId);

        bool IncreasingFollowersCount(string subCategoryId);

        bool DecreasingFollowersCount(string subCategoryId);

        SubCategoryDetailInfoModel GetSubCategoryDetail(string subCategoryId, Languages language);
    }
}