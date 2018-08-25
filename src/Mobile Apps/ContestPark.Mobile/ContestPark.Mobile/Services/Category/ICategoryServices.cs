using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Category
{
    public interface ICategoryServices
    {
        Task<ServiceModel<CategoryModel>> CategoryListAsync(PagingModel pagingModel);
    }
}