using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Follow.API.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Follow.API.Infrastructure.MySql.Repositories
{
    public class FollowRepository : IFollowRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Follow> _followRepository;
        private readonly ILogger<FollowRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public FollowRepository(IRepository<Tables.Follow> followRepository,
                                 ILogger<FollowRepository> logger)
        {
            _followRepository = followRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcıyı takip edenlerin listesini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Takipçi listesi</returns>
        public ServiceModel<FollowUserModel> Followers(string userId, PagingModel paging)
        {
            string sql = @"SELECT
                           f.FollowUpUserId AS UserId,

                           (SELECT (CASE
                           WHEN EXISTS(
                           SELECT NULL
                           FROM Follows fstatus WHERE fstatus.FollowUpUserId = f.FollowUpUserId AND fstatus.FollowedUserId = f.FollowedUserId)
                           THEN 1
                           ELSE 0
                           END)) AS IsFollow

                           FROM Follows f
                           WHERE f.FollowedUserId = @userId";

            return _followRepository.ToServiceModel<FollowUserModel>(sql, new
            {
                userId,
            }, pagingModel: paging);
        }

        /// <summary>
        /// Kullanıcının takip ettiklerini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Takip ettikleri</returns>
        public ServiceModel<FollowUserModel> Following(string userId, PagingModel paging)
        {
            string sql = @"SELECT
                           f.FollowedUserId AS UserId,

                           (SELECT (CASE
                           WHEN EXISTS(
                           SELECT NULL
                           FROM Follows fstatus WHERE fstatus.FollowUpUserId = f.FollowUpUserId AND fstatus.FollowedUserId = f.FollowedUserId)
                           THEN 1
                           ELSE 0
                           END)) AS IsFollow

                           FROM Follows f
                           WHERE f.FollowUpUserId = @userId";

            return _followRepository.ToServiceModel<FollowUserModel>(sql, new
            {
                userId,
            }, pagingModel: paging);
        }

        /// <summary>
        /// Kullanıcının takip ettiği user id'lerini verir
        /// bu kısım duel servisinde takip ettiklerim sıralaması için eklendi
        /// </summary>
        /// <param name="userId">Takip eden kullanıcı id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Takip edilen kullanıcı idleri</returns>
        public IEnumerable<string> GetFollowingUserIds(string userId, PagingModel paging)
        {
            string sql = @"SELECT
                           f.FollowedUserId AS UserId
                           FROM Follows f
                           WHERE f.FollowUpUserId = @userId
                           LIMIT @Offset, @PageSize";

            return _followRepository.QueryMultiple<string>(sql, new
            {
                userId,
                paging.PageSize,
                paging.Offset
            });
        }

        /// <summary>
        /// Takip etme durumunu verir
        /// </summary>
        /// <param name="FollowUpUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        /// <returns>Takip ediyorsa true etmiyorsa false</returns>
        public bool CheckFollowUpStatus(string FollowUpUserId, string followedUserId)
        {
            string sql = $@"(SELECT (CASE
                            WHEN EXISTS(
                            SELECT NULL
                            FROM Follows fstatus WHERE fstatus.FollowUpUserId = @FollowUpUserId AND fstatus.FollowedUserId = @followedUserId)
                            THEN 1
                            ELSE 0
                            END))";

            return _followRepository.QuerySingleOrDefault<bool>(sql,
                     new
                     {
                         FollowUpUserId,
                         followedUserId
                     });
        }

        /// <summary>
        /// Kullanıcıyı takip et
        /// </summary>
        /// <param name="followUpUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> FollowAsync(string followUpUserId, string followedUserId)
        {
            int? followId = await _followRepository.AddAsync(new Tables.Follow
            {
                FollowUpUserId = followUpUserId,
                FollowedUserId = followedUserId
            });

            return followId.HasValue;
        }

        /// <summary>
        /// Takipten çıkar
        /// </summary>
        /// <param name="followUpUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        /// <returns>Takip ediyorsa true etmiyorsa false</returns>
        public Task<bool> UnFollowAsync(string followUpUserId, string followedUserId)
        {
            return _followRepository.ExecuteAsync(@"DELETE FROM Follows f
                                                    WHERE f.FollowUpUserId = @followUpUserId AND f.FollowedUserId = @followedUserId", new
            {
                followUpUserId,
                followedUserId
            });
        }

        #endregion Methods
    }
}
