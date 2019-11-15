using ContestPark.Admin.API.Model.Category;
using ContestPark.Admin.API.Model.SubCategory;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.SubCategory
{
    public interface ISubCategoryRepository
    {
        ServiceModel<CategoryDropdownModel> GetSubCategoryDropList(Languages language, PagingModel paging);

        ServiceModel<SubCategoryModel> GetSubCategories(Languages language, PagingModel paging);

        Task<short?> InsertAsync(SubCategoryInsertModel subCategoryInsert);

        Task<bool> UpdateAsync(SubCategoryUpdateModel subCategoryUpdate);

        SubCategoryUpdateModel GetSubCategoryById(short subCategoryId);
    }
}
