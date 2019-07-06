using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory
{
    public interface IFollowSubCategoryRepository
    {
        bool IsSubCategoryFollowed(string userId, short subCategoryId);

        Task<bool> UnFollowSubCategoryAsync(string userId, short subCategoryId);

        Task<bool> FollowSubCategoryAsync(string userId, short subCategoryId);

        IEnumerable<short> FollowedSubCategoryIds(string userId);
    }
}
