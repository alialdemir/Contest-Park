using ContestPark.Category.API.Models;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory
{
    public interface IFollowSubCategoryRepository
    {
        bool IsSubCategoryFollowed(string userId, short subCategoryId);

        Task<bool> UnFollowSubCategoryAsync(string userId, short subCategoryId);

        Task<bool> FollowSubCategoryAsync(string userId, short subCategoryId);

        ServiceModel<SearchModel> FollowedSubCategoryIds(string searchText, string userId, Languages language, PagingModel pagingModel);

        Task<bool> FollowSubCategoryAsync(string userId, IEnumerable<short> subCategoryIds);
    }
}
