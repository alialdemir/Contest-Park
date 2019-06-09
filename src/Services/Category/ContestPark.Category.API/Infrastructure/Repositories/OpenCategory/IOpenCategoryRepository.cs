using ContestPark.Category.API.Infrastructure.Documents;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.OpenCategory
{
    public interface IOpenCategoryRepository
    {
        string[] OpenSubCategoryIds(string userId);

        bool IsSubCategoryOpen(string userId, string subCategoryId);

        Task<bool> AddAsync(OpenSubCategory openSubCategory);
    }
}