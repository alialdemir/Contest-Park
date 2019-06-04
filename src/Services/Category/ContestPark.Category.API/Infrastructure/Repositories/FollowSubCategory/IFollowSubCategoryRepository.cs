using ContestPark.Core.CosmosDb.Interfaces;

namespace ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory
{
    public interface IFollowSubCategoryRepository
    {
        IDocumentDbRepository<Documents.FollowSubCategory> Repository { get; }

        bool IsSubCategoryFollowed(string userId, string subCategoryId);
    }
}