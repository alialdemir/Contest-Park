using ContestPark.Core.Enums;
using ContestPark.Duel.API.Models;
using ContestPark.Duel.API.Services.SubCategory;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.FunctionalTests
{
    public class SubCategoryMockService : ISubCategoryService
    {
        public Task<SubCategoryModel> GetSubCategoryInfo(short subCategoryId, Languages language)
        {
            if (language == Languages.Turkish)
            {
                return Task.FromResult(new SubCategoryModel
                {
                    SubCategoryName = "Hakem",
                    SubCategoryPicturePath = "https://static.thenounproject.com/png/14039-200.png"
                });
            }

            return Task.FromResult(new SubCategoryModel
            {
                SubCategoryName = "Refree",
                SubCategoryPicturePath = "https://static.thenounproject.com/png/14039-200.png"
            });
        }
    }
}
