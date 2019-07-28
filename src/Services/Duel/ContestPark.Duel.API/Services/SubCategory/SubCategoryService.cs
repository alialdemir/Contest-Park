using ContestPark.Core.Enums;
using ContestPark.Core.Services.RequestProvider;
using ContestPark.Duel.API.Models;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Services.SubCategory
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly IRequestProvider _requestProvider;

        private readonly DuelSettings _categorySettings;

        public SubCategoryService(IRequestProvider requestProvider,
                                  IOptions<DuelSettings> settings)
        {
            _requestProvider = requestProvider ?? throw new ArgumentNullException(nameof(requestProvider));
            _categorySettings = settings.Value;
        }

        public Task<SubCategoryModel> GetSubCategoryInfo(short subCategoryId, Languages language)
        {
            string url = $"{_categorySettings.SubCategoryUrl}/api/v1/SubCategory/{subCategoryId}/Info?language={(byte)language}";

            return _requestProvider.GetAsync<SubCategoryModel>(url);
        }
    }
}
