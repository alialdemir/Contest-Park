using ContestPark.Core.CosmosDb.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
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

            var following = _repository.GetById(followingUserId);
            if (following == null)// İlk takip işleminde update yapabilmek için extradan bir insert atacak
            {
                following = new Documents.Follow { Id = followingUserId };
                bool isRegistered = await _repository.InsertAsync(following);
                if (!isRegistered)
                    return false;
            }

            var followed = _repository.GetById(followedUserId);
            if (followed == null)// İlk takip işleminde update yapabilmek için extradan bir insert atacak
            {
                followed = new Documents.Follow { Id = followedUserId };
                bool isRegistered = await _repository.InsertAsync(followed);
                if (!isRegistered)
                    return false;
            }

            bool isAdded = !following.Following.Any(userId => userId == followedUserId) && !followed.Followers.Any(userId => userId == followingUserId);
            if (isAdded)
            {
                following.Following.Add(followedUserId);

                isAdded = await _repository.UpdateAsync(following);
            }

            if (isAdded)
            {
                followed.Followers.Add(followingUserId);

                bool isSuccess = await _repository.UpdateAsync(followed);
                if (!isSuccess)
                {
                    _logger.LogCritical($@"CRITICAL ERROR: Takip etme sırasında ilk kullanıcıya kayıt eklendi fakat ikinci kullanıcıya followers listesine eklenemedi.
                                            Lütfen kontrol edin followingUserId: {followingUserId} followedUserId:{followedUserId}");
                    // TODO: following.Following içinden followedUserId sil
                    return false;
                }
            }

            return isAdded;
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
                QueryText = "SELECT DISTINCT VALUE ARRAY_CONTAINS(c.following, @followedUserId, true) FROM c WHERE c.id=@followingUserId ",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@followingUserId", followingUserId),
                    new SqlParameter("@followedUserId", followedUserId)
                }
            }).ToList().FirstOrDefault();
        }

        #endregion Methods
    }
}