using ContestPark.Core.CosmosDb.Interfaces;
using Microsoft.Azure.Documents;
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
        private readonly IDocumentDbRepository<Documents.Follow> _repository;

        #endregion Private Variables

        #region Constructor

        public FollowRepository(IDocumentDbRepository<Documents.Follow> repository,
                                ILogger<FollowRepository> logger)
        {
            _repository = repository;
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

                isAddedSuccess = await _repository.UpdateAsync(following);
            }

            if (isAddedSuccess)
            {
                followed.Followers.Add(followingUserId);

                isAddedSuccess = await _repository.UpdateAsync(followed);
                if (!isAddedSuccess)
                {
                    _logger.LogCritical($@"CRITICAL ERROR: Takip etme sırasında ilk kullanıcıya kayıt eklendi fakat ikinci kullanıcıya followers listesine eklenemedi.
                                            Lütfen kontrol edin followingUserId: {followingUserId} followedUserId:{followedUserId}");
                    // TODO: following.Following içinden followedUserId sil
                    return false;
                }
            }

            return isAddedSuccess;
        }

        /// <summary>
        /// Kullanıcıyı takip edenlerin listesini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Takipçi listesi</returns>
        public string[] Followers(string userId)
        {
            return _repository.Query<string>(new SqlQuerySpec
            {
                QueryText = "SELECT DISTINCT VALUE f FROM c JOIN f IN c.followers WHERE c.id=@userId",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@userId", userId),
                }
            }).ToArray();
        }

        /// <summary>
        /// Kullanıcının takip ettiklerini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Takip ettikleri</returns>
        public string[] Following(string userId)
        {
            return _repository.Query<string>(new SqlQuerySpec
            {
                QueryText = "SELECT DISTINCT VALUE f FROM c JOIN f IN c.following WHERE c.id=@userId",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@userId", userId),
                }
            }).ToArray();
        }

        /// <summary>
        /// Takip etme durumunu verir
        /// </summary>
        /// <param name="followingUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        /// <returns>Takip ediyorsa true etmiyorsa false</returns>
        public bool IsFollowUpStatus(string followingUserId, string followedUserId)
        {
            return _repository.Query<bool>(new SqlQuerySpec
            {
                QueryText = "SELECT DISTINCT VALUE ARRAY_CONTAINS(c.following, @followedUserId, true) FROM c WHERE c.id=@followingUserId",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@followingUserId", followingUserId),
                    new SqlParameter("@followedUserId", followedUserId)
                }
            }).ToList().FirstOrDefault();
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

            bool isAddedSuccess = await _repository.UpdateAsync(following);

            if (isAddedSuccess)
            {
                followed.Followers.Remove(followingUserId);

                isAddedSuccess = await _repository.UpdateAsync(followed);
                if (!isAddedSuccess)
                {
                    _logger.LogCritical($@"CRITICAL ERROR: Takipten çıkarma sırasında ilk kullanıcıya kayıt eklendi fakat ikinci kullanıcıya followers listesine eklenemedi.
                                            Lütfen kontrol edin followingUserId: {followingUserId} followedUserId:{followedUserId}");
                    // TODO: following.Following içinden followedUserId sil
                    return false;
                }
            }

            return isAddedSuccess;
        }

        /// <summary>
        /// user id ait kayıt varsa onu döndürür yoksa yenisini insert edip döndürür
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns></returns>
        private async Task<Documents.Follow> GetFollowIfNotExistsCreateAsync(string userId)
        {
            var follow = _repository.GetById(userId);
            if (follow == null)// İlk takip işleminde update yapabilmek için extradan bir insert atacak
            {
                follow = new Documents.Follow { Id = userId };
                await _repository.InsertAsync(follow);
            }

            return follow;
        }

        #endregion Methods
    }
}