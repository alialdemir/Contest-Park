using ContestPark.Core.Enums;
using ContestPark.Core.Services.RequestProvider;
using ContestPark.Duel.API.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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

        public Task<SubCategoryModel> GetSubCategoryInfo(short subCategoryId, Languages language, string userId)
        {
            string url = $"{_categorySettings.SubCategoryUrl}/api/v1/SubCategory/{subCategoryId}/Info?language={(byte)language}&userId={userId}";

            return _requestProvider.GetAsync<SubCategoryModel>(url);
        }

        /// <summary>
        /// Kullanıcı idlerinin ilgili kategorideki leveli
        /// </summary>
        /// <param name="userIds">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Kullanıcının o alt kategorideki leveli</returns>
        public Task<IEnumerable<UserLevelModel>> UserLevel(IEnumerable<string> userIds, short subCategoryId)
        {
            string url = $"{_categorySettings.SubCategoryUrl}/api/v1/SubCategory/{subCategoryId}/UserLevel";

            return _requestProvider.PostAsync<IEnumerable<UserLevelModel>>(url, userIds);
        }
    }
}
