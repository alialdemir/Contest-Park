using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.OpenSubCategory
{
    public interface IOpenCategoryRepository
    {
        bool IsSubCategoryOpen(string userId, short subCategoryId);

        Task<bool> UnLockSubCategory(Tables.OpenSubCategory openSubCategory);
    }
}
