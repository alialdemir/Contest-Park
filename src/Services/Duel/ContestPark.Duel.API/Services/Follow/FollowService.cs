using ContestPark.Core.Database.Models;
using ContestPark.Core.Services.RequestProvider;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Services.Follow
{
    public class FollowService : IFollowService
    {
        private readonly IRequestProvider _requestProvider;

        private readonly DuelSettings _categorySettings;

        public FollowService(IRequestProvider requestProvider,
                             IOptions<DuelSettings> settings)
        {
            _requestProvider = requestProvider ?? throw new ArgumentNullException(nameof(requestProvider));
            _categorySettings = settings.Value;
        }

        public Task<IEnumerable<string>> GetFollowingUserIds(string userId, PagingModel pagingModel)
        {
            string url = $"{_categorySettings.FollowUrl}/api/v1/Follow/{userId}/FollowingUserIds{pagingModel}";

            return _requestProvider.GetAsync<IEnumerable<string>>(url);
        }
    }
}
