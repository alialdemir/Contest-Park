using ContestPark.Core.Services.RequestProvider;
using ContestPark.Identity.API.Models;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services.Follow
{
    public class FollowService : IFollowService
    {
        private readonly IRequestProvider _requestProvider;

        private readonly IdentitySettings _identitySettings;

        public FollowService(IRequestProvider requestProvider,
                              IOptions<IdentitySettings> settings)
        {
            _requestProvider = requestProvider ?? throw new ArgumentNullException(nameof(requestProvider));
            _identitySettings = settings.Value;
        }

        /// <summary>
        /// İki kullanıcı arasındaki engelleme durumunu verir
        /// </summary>
        /// <param name="userId1">Kullanıcı id</param>
        /// <param name="userId2">Kullanıcı id</param>
        /// <returns>Herhangi biri engellemiş ise true engellememiş ise false</returns>
        public async Task<bool> FollowStatusAsync(string userId1, string userId2)
        {
            string url = $"{_identitySettings.FollowUrl}/api/v1/Follow/Status/{userId1}/{userId2}";

            StatusModel result = await _requestProvider.GetAsync<StatusModel>(url);

            return result != null && result.IsStatus;
        }
    }
}
