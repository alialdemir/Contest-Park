using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Follow.API.Infrastructure.Documents;
using ContestPark.Follow.API.Infrastructure.Repositories.Follow;
using ContestPark.Follow.API.Models;
using ContestPark.Follow.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Follow.API.Controllers
{
    public class FollowController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IFollowRepository _followRepository;
        private readonly IDocumentDbRepository<User> _userRepository;

        #endregion Private Variables

        #region Constructor

        public FollowController(IFollowRepository followRepository,
                                IDocumentDbRepository<User> userRepository,
                                ILogger<FollowController> logger) : base(logger)
        {
            _followRepository = followRepository ?? throw new ArgumentNullException(nameof(followRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Parametreden gelen kullanıcıyı takip et
        /// </summary>
        /// <returns>Başarılı ise OK değilse BadRequest mesajı döndürür.</returns>
        [HttpPost("{followedUserId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post(string followedUserId)
        {
            if (string.IsNullOrEmpty(followedUserId) || UserId == followedUserId)
                return BadRequest();

            if (_followRepository.IsFollowUpStatus(UserId, followedUserId))
                return BadRequest(FollowResource.YouAreAlreadyFollowingThisUser);

            // TODO: notification
            // TODO: post

            bool isSuccess = await _followRepository.FollowAsync(UserId, followedUserId);
            if (!isSuccess)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Parametreden gelen kullanıcıyı takipten çıkar
        /// </summary>
        /// <returns>Başarılı ise OK değilse BadRequest mesajı döndürür.</returns>
        [HttpDelete("{followedUserId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(string followedUserId)
        {
            if (string.IsNullOrEmpty(followedUserId) || UserId == followedUserId)
                return BadRequest();

            if (!_followRepository.IsFollowUpStatus(UserId, followedUserId))
                return BadRequest(FollowResource.YouAreNotFollowingThisUser);

            bool isSuccess = await _followRepository.UnFollowAsync(UserId, followedUserId);
            if (!isSuccess)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Parametreden gelen kullanıcının takip ettikleri
        /// </summary>
        /// <returns>Başarılı ise OK değilse BadRequest mesajı döndürür.</returns>
        [HttpGet("{userId}/Following")]
        [ProducesResponseType(typeof(List<FollowModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetFollowing(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            string[] following = _followRepository.Following(userId);
            if (following.Length == 0)
                return NotFound();

            string[] currentUserFollowers = _followRepository.Following(UserId);
            IEnumerable<User> followingUsers = _userRepository.GetByIds(following);

            List<FollowModel> follows = (from f in following
                                         from u in followingUsers
                                         where f == u.Id
                                         select new FollowModel
                                         {
                                             UserId = u.Id,
                                             IsFollowing = currentUserFollowers.Any(x => x == f),
                                             FullName = u.FullName,
                                             ProfilePicturePath = u.ProfilePicturePath,
                                             UserName = u.UserName,
                                         }).ToList();

            // TODO: eğer kullanıcı tablosunda yoksa identity apiden olmayan kullanıcıları istemeli istemeli

            //if (following.Count() != followingUsers.Count())
            //{
            //    // Kullanıcı tablosunsa olmayan kullanıcılar
            //    string[] notFoundUsers = following.Where(p => !followingUsers.Any(x => x.Id == p)).ToArray();
            //}

            return Ok(follows);
        }

        #endregion Methods
    }
}