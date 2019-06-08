using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Core.CosmosDb.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Follow.API.Infrastructure.Repositories.Follow
{
    public class FollowRepository : IFollowRepository
    {
        #region Private Variables

        private readonly ILogger<FollowRepository> _logger;
        private readonly IDocumentDbRepository<Documents.Follow> _followRepository;

        #endregion Private Variables

        #region Constructor

        public FollowRepository(IDocumentDbRepository<Documents.Follow> followRepository,
                                ILogger<FollowRepository> logger)
        {
            _followRepository = followRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcıyı takip et
        /// </summary>
        /// <param name="followingUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> FollowAsync(string followingUserId, string followedUserId)
        {
            if (followingUserId == followedUserId)
                return false;

            var following = await GetFollowIfNotExistsCreateAsync(followingUserId);
            var followed = await GetFollowIfNotExistsCreateAsync(followedUserId);

            bool isAddedSuccess = !following.Following.Any(userId => userId == followedUserId) && !followed.Followers.Any(userId => userId == followingUserId);
            if (isAddedSuccess)
            {
                following.Following.Add(followedUserId);

                followed.Followers.Add(followingUserId);

                isAddedSuccess = await _followRepository.UpdateRangeAsync(new List<Documents.Follow>
                {
                    following,
                    followed
                });

                if (!isAddedSuccess)
                {
                    _logger.LogCritical($@"CRITICAL ERROR: Takip etme sırasında ilk kullanıcıya kayıt eklendi fakat ikinci kullanıcıya followers listesine eklenemedi.
                                            Lütfen kontrol edin followingUserId: {followingUserId} followedUserId:{followedUserId}");
                    // TODO: following.Following içinden followedUserId sil
                }
            }

            return isAddedSuccess;
        }

        /// <summary>
        /// Kullanıcıyı takip edenlerin listesini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Takipçi listesi</returns>
        public IEnumerable<string> Followers(string userId, PagingModel paging)
        {
            return _followRepository.QueryMultipleAsync<string>("SELECT value f  FROM c JOIN f IN c.Followers WHERE c.id=@userId OFFSET @pageNumber LIMIT @pageSize",
                new
                {
                    userId,
                    pageSize = paging.PageSize,
                    pageNumber = paging.PageNumber - 1
                });
        }

        /// <summary>
        /// Kullanıcının takip ettiklerini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Takip ettikleri</returns>
        public IEnumerable<string> Following(string userId, PagingModel paging)
        {
            return _followRepository.QueryMultipleAsync<string>("SELECT value f  FROM c JOIN f IN c.Following WHERE c.id=@userId OFFSET @pageNumber LIMIT @pageSize",
                   new
                   {
                       userId,
                       pageSize = paging.PageSize,
                       pageNumber = paging.PageNumber - 1
                   });
        }

        /// <summary>
        /// Takip etme durumunu verir
        /// </summary>
        /// <param name="followingUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        /// <returns>Takip ediyorsa true etmiyorsa false</returns>
        public bool CheckFollowUpStatus(string followingUserId, string followedUserId)
        {
            return CheckFollowUpStatus(followingUserId, new List<string> { followedUserId })?.Count() == 1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="followingUserId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public IEnumerable<string> CheckFollowUpStatus(string followingUserId, IEnumerable<string> userIds)
        {
            return _followRepository.QueryMultipleAsync<string>("SELECT value f FROM c JOIN f IN c.Following  WHERE c.id=@followingUserId AND ARRAY_CONTAINS(@userIds, f)",
                     new
                     {
                         followingUserId,
                         userIds
                     });
        }

        /// <summary>
        /// Takipten çıkar
        /// </summary>
        /// <param name="followingUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        /// <returns>Takip ediyorsa true etmiyorsa false</returns>
        public async Task<bool> UnFollowAsync(string followingUserId, string followedUserId)
        {
            if (followingUserId == followedUserId)
                return false;

            var following = await GetFollowIfNotExistsCreateAsync(followingUserId);
            var followed = await GetFollowIfNotExistsCreateAsync(followedUserId);

            following.Following.Remove(followedUserId);

            followed.Followers.Remove(followingUserId);

            bool isSuccess = await _followRepository.UpdateRangeAsync(new List<Documents.Follow>
            {
                following,
                followed
            });

            if (!isSuccess)
            {
                _logger.LogCritical($@"CRITICAL ERROR: Takipten çıkarma sırasında ilk kullanıcıya kayıt eklendi fakat ikinci kullanıcıya followers listesine eklenemedi.
                                            Lütfen kontrol edin followingUserId: {followingUserId} followedUserId:{followedUserId}");
                // TODO: following.Following içinden followedUserId sil
            }

            return isSuccess;
        }

        /// <summary>
        /// user id ait kayıt varsa onu döndürür yoksa yenisini insert edip döndürür
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns></returns>
        private async Task<Documents.Follow> GetFollowIfNotExistsCreateAsync(string userId)
        {
            var follow = _followRepository.FindById(userId);
            if (follow == null)// İlk takip işleminde update yapabilmek için extradan bir insert atacak
            {
                follow = new Documents.Follow { Id = userId };
                await _followRepository.AddAsync(follow);
            }

            return follow;
        }

        #endregion Methods
    }
}