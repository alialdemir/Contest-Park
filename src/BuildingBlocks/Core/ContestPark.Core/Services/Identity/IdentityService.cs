using ContestPark.Core.Models;
using ContestPark.Core.Services.RequestProvider;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Core.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        #region Private Variables

        private readonly IRequestProvider _requestProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly string baseUrl = "";

        #endregion Private Variables

        #region Constructor

        public IdentityService(IRequestProvider requestProvider,
                               IConfiguration configuration,
                               IMemoryCache memoryCache)
        {
            _requestProvider = requestProvider ?? throw new ArgumentNullException(nameof(requestProvider));
            _memoryCache = memoryCache;
            baseUrl = configuration["identityUrl"] ?? throw new ArgumentNullException(nameof(baseUrl));
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı idlerine ait adsoyad, kullanıcı adı, profil resmi, user id bilgilerini döner
        /// </summary>
        /// <param name="userIds">Kullanıcı idleri</param>
        /// <returns>Kullanıcı bilgilei</returns>
        public async Task<IEnumerable<UserModel>> GetUserInfosAsync(IEnumerable<string> userIds)
        {
            var result = await _memoryCache.GetOrCreate("UserInfos", async (entry) =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(10);
                return await _requestProvider.PostAsync<IEnumerable<UserModel>>($"{baseUrl}/api/v1/account/UserInfos", userIds);
            });

            return result;
        }

        #endregion Methods
    }
}
