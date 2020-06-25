using ContestPark.Core.Enums;
using ContestPark.Duel.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Services.SubCategory
{
    public interface ISubCategoryService
    {
        Task<SubCategoryModel> GetSubCategoryInfo(short subCategoryId, Languages language, string userId);
        Task<IEnumerable<UserLevelModel>> UserLevel(IEnumerable<string> userIds, short subCategoryId);
    }
}
