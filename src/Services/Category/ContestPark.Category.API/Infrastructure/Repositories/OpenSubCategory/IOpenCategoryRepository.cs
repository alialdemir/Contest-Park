using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.OpenSubCategory
{
    public interface IOpenCategoryRepository
    {
        bool IsSubCategoryOpen(string userId, short subCategoryId);

        List<short> IsSubCategoryOpen(string userId, IEnumerable<short> subCategoryIds);

        Task<bool> UnLockSubCategory(Tables.OpenSubCategory openSubCategory);
    }
}
