using ContestPark.Admin.API.Model.Category;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.Category
{
    public interface ICategoryRepository
    {
        ServiceModel<CategoryModel> GetCategories(Languages language, PagingModel paging);

        CategoryUpdateModel GetCategoryById(short categoryId);
        Task<bool> InsertAsync(CategoryInsertModel categoryInsert);
        Task<bool> UpdateAsync(CategoryUpdateModel categoryUpdate);
    }
}
