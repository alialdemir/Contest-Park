using ContestPark.Core.Domain.Interfaces;
using ContestPark.Core.Domain.Model;
using ContestPark.Core.Enums;
using System.Threading.Tasks;

namespace ContestPark.Domain.Category.Interfaces
{
    public interface ICategoryGrain : IGrainBase
    {
        Task<ServiceResponse<Model.Response.Category>> GetCategoryList(string userId, Languages language, Paging paging);
    }
}