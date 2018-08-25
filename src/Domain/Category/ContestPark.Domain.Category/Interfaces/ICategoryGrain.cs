using ContestPark.Core.Domain.Model;
using ContestPark.Core.Enums;
using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Domain.Category.Interfaces
{
    public interface ICategoryGrain : IGrainWithIntegerKey
    {
        Task<ServiceResponse<Model.Response.Category>> GetCategoryList(string userId, Languages language, Paging paging);
    }
}