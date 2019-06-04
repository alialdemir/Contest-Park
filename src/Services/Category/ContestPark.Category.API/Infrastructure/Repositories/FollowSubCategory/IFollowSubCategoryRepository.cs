using ContestPark.Core.CosmosDb.Interfaces;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory
{
    public interface IFollowSubCategoryRepository
    {
        IDocumentDbRepository<Documents.FollowSubCategory> Repository { get; }

        bool IsSubCategoryFollowed(string userId, string subCategoryId);

        string[] FollowedSubCategoryIds(string userId);

        Task<bool> DeleteAsync(string userId, string subCategoryId);
    }
}