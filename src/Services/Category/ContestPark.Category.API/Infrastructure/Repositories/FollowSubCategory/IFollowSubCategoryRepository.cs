using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory
{
    public interface IFollowSubCategoryRepository
    {
        bool IsSubCategoryFollowed(string userId, string subCategoryId);

        string[] FollowedSubCategoryIds(string userId);

        Task<bool> DeleteAsync(string userId, string subCategoryId);

        Task<bool> AddAsync(Documents.FollowSubCategory followSubCategory);
    }
}