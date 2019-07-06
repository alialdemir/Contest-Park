using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Follow.API.Infrastructure.Documents;
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
        private readonly IRepository<Documents.Follow> _followRepository;
        private readonly IRepository<Documents.User> _userRepository;

        #endregion Private Variables

        #region Constructor

        public FollowRepository(IRepository<Documents.Follow> followRepository,
                                IRepository<Documents.User> userRepository,
                                ILogger<FollowRepository> logger)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcıyı takip edenlerin listesini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Takipçi listesi</returns>
        public ServiceModel<string> Followers(string userId, PagingModel paging)
        {
            string sql = "SELECT VALUE c.FollowUpUserId FROM c WHERE c.FollowedUserId=@userId";
            return _followRepository.ToServiceModel<Documents.Follow, string>(sql, new
            {
                userId,
            }, paging);
        }

        /// <summary>
        /// Kullanıcının takip ettiklerini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Takip ettikleri</returns>
        public ServiceModel<string> Following(string userId, PagingModel paging)
        {
            string sql = "SELECT VALUE c.FollowedUserId FROM c WHERE c.FollowUpUserId=@userId";
            return _followRepository.ToServiceModel<Documents.Follow, string>(sql, new
            {
                userId,
            }, paging);
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
        /// Kullanıcı parametreden gelen kullanıcı idlerini takip ediyor mu onu döndürür
        /// </summary>
        /// <param name="followingUserId"></param>
        /// <param name="userIds">Kullanıcı idleri</param>
        /// <returns>Takip etme durumları</returns>
        public IEnumerable<string> CheckFollowUpStatus(string followingUserId, IEnumerable<string> userIds)
        {
            string sql = @"SELECT VALUE c.FollowedUserId
                           FROM c
                           WHERE c.FollowUpUserId=@followingUserId AND ARRAY_CONTAINS(@userIds, c.FollowedUserId)";
            return _followRepository.QueryMultiple<string>(sql,
                     new
                     {
                         followingUserId,
                         userIds
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
            if (followUpUserId == followedUserId)
                return false;

            User followUpUser = _userRepository.FindById(followUpUserId);
            User followedUser = _userRepository.FindById(followedUserId);
            if (followUpUser == null || followedUser == null)
            {
                _logger.LogCritical($"CRITICAL: Takip etmek istenilen kullanıcılardan biri user toplasında bulunamadı", followUpUser, followedUser);

                return false;
            }

            Documents.Follow follow = new Documents.Follow
            {
                FollowedUserId = followedUserId,
                FollowUpUserId = followUpUserId
            };
            bool isSuccess = await _followRepository.AddAsync(follow);
            if (!isSuccess)
            {
                _logger.LogCritical($"Takip etme sırasında hata oluştu. followUpUserId: {followUpUserId} followedUserId: {followedUserId}");

                return false;
            }

            followUpUser.FollowingCount += 1;
            followedUser.FollowersCount += 1;

            isSuccess = await _userRepository.UpdateRangeAsync(new List<User>
            {
                followUpUser,
                followedUser
            });

            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: Kullanıcının takip ettiği kişi takipçiler documentine eklendi ancak Kullanıcı takipçi sayısı güncellenemedi",
                                    followedUserId,
                                    followUpUserId);
                // TODO: burada ya takipçi sayılarını güncellemek için event yollanabilir yada işlem geri alınmalı
            }

            return true;
        }

        /// <summary>
        /// Takipten çıkar
        /// </summary>
        /// <param name="followUpUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        /// <returns>Takip ediyorsa true etmiyorsa false</returns>
        public async Task<bool> UnFollowAsync(string followUpUserId, string followedUserId)
        {
            if (followUpUserId == followedUserId)
                return false;

            User followUpUser = _userRepository.FindById(followUpUserId);
            User followedUser = _userRepository.FindById(followedUserId);
            if (followUpUser == null || followedUser == null)
            {
                _logger.LogCritical($"CRITICAL: Takipten çıkarılmak istenilen kullanıcılardan biri user toplasında bulunamadı", followUpUser, followedUser);

                return false;
            }

            bool isSuccess = await _followRepository.RemoveAsync(f => f.FollowedUserId == followedUserId && f.FollowUpUserId == followUpUserId);
            if (!isSuccess)
            {
                _logger.LogCritical($"Takipten çıkarma sırasında hata oluştu. followUpUserId: {followUpUserId} followedUserId: {followedUserId}");

                return false;
            }

            followUpUser.FollowingCount -= 1;
            followedUser.FollowersCount -= 1;

            isSuccess = await _userRepository.UpdateRangeAsync(new List<User>
            {
                followUpUser,
                followedUser
            });

            if (!isSuccess)
            {
                // TODO: burada ya takipçi sayılarını güncellemek için event yollanabilir yada işlem geri alınmalı
            }

            return isSuccess;
        }

        #endregion Methods
    }
}
