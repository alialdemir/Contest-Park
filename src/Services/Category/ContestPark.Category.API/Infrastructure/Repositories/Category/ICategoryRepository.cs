using ContestPark.Category.API.Model;
using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.Enums;

namespace ContestPark.Category.API.Infrastructure.Repositories.Category
{
    public interface ICategoryRepository
    {
        IDocumentDbRepository<Documents.Category> Repository { get; }

        ServiceModel<CategoryModel> GetCategories(string userId, Languages language, PagingModel pagingModel);

        bool IsSubCategoryFree(string subCategoryId);

        bool IncreasingFollowersCount(string subCategoryId);
    }
}