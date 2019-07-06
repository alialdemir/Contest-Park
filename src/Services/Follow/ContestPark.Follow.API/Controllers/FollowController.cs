using ContestPark.Core.Database.Models;
using ContestPark.Core.Models;
using ContestPark.Core.Services.Identity;
using ContestPark.EventBus.Abstractions;
using ContestPark.Follow.API.Infrastructure.MySql.Repositories;
using ContestPark.Follow.API.IntegrationEvents.Events;
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
        private readonly IEventBus _eventBus;
        private readonly IIdentityService _identityService;

        #endregion Private Variables

        #region Constructor

        public FollowController(IFollowRepository followRepository,
                                IEventBus eventBus,
                                IIdentityService identityService,
                                ILogger<FollowController> logger) : base(logger)
        {
            _followRepository = followRepository ?? throw new ArgumentNullException(nameof(followRepository));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _identityService = identityService;
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
        public async Task<IActionResult> Post([FromRoute]string followedUserId)
        {
            if (string.IsNullOrEmpty(followedUserId) || UserId == followedUserId)
                return BadRequest();

            if (_followRepository.CheckFollowUpStatus(UserId, followedUserId))
                return BadRequest(FollowResource.YouAreAlreadyFollowingThisUser);

            bool isSuccess = await _followRepository.FollowAsync(UserId, followedUserId);
            if (!isSuccess)
                return BadRequest();

            // Kullanıcıların takipçi sayısının değiştiğini diğer servise bildirdik
            var @event = new FollowIntegrationEvent(UserId, followedUserId);
            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Parametreden gelen kullanıcıyı takipten çıkar
        /// </summary>
        /// <returns>Başarılı ise OK değilse BadRequest mesajı döndürür.</returns>
        [HttpDelete("{followedUserId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]string followedUserId)
        {
            if (string.IsNullOrEmpty(followedUserId) || UserId == followedUserId)
                return BadRequest();

            if (!_followRepository.CheckFollowUpStatus(UserId, followedUserId))
                return BadRequest(FollowResource.YouAreNotFollowingThisUser);

            bool isSuccess = await _followRepository.UnFollowAsync(UserId, followedUserId);
            if (!isSuccess)
                return BadRequest();

            // Kullanıcıların takipçi sayısının değiştiğini diğer servise bildirdik
            var @event = new UnFollowIntegrationEvent(UserId, followedUserId);
            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Parametreden gelen kullanıcının takip ettikleri
        /// </summary>
        /// <returns>Başarılı ise OK değilse BadRequest mesajı döndürür.</returns>
        [HttpGet("{userId}/Following")]
        [ProducesResponseType(typeof(ServiceModel<FollowModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetFollowing(string userId, [FromQuery]PagingModel pagingModel)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            ServiceModel<FollowUserModel> following = _followRepository.Following(userId, pagingModel);
            if (following.Items.Count() == 0)
                return NotFound();

            ServiceModel<FollowModel> result = await GetFollowServiceModel(following, pagingModel);

            return Ok(result);
        }

        /// <summary>
        /// Parametreden gelen kullanıcının takip edenler
        /// </summary>
        /// <returns>Başarılı ise OK değilse BadRequest mesajı döndürür.</returns>
        [HttpGet("{userId}/Followers")]
        [ProducesResponseType(typeof(ServiceModel<FollowModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetFollowers(string userId, [FromQuery]PagingModel pagingModel)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            ServiceModel<FollowUserModel> followers = _followRepository.Followers(userId, pagingModel);
            if (followers.Items.Count() == 0)
                return NotFound();

            ServiceModel<FollowModel> result = await GetFollowServiceModel(followers, pagingModel);

            return Ok(result);
        }

        /// <summary>
        /// Takip listeesindeki kullanıcıların bilgileri Identity servisinden alıp birleştirerek döndürür
        /// </summary>
        /// <param name="followers"></param>
        /// <param name="pagingModel"></param>
        /// <returns></returns>
        private async Task<ServiceModel<FollowModel>> GetFollowServiceModel(ServiceModel<FollowUserModel> followers,
                                                                PagingModel pagingModel)
        {
            IEnumerable<UserModel> followingUsers = await _identityService.GetUserInfosAsync(followers.Items.Select(x => x.UserId).ToList());

            return new ServiceModel<FollowModel>
            {
                HasNextPage = followers.HasNextPage,
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize,
                Items = from follower in followers.Items
                        join followingUser in followingUsers on follower.UserId equals followingUser.UserId
                        select new FollowModel
                        {
                            UserId = follower.UserId,
                            IsFollowing = follower.IsFollow,
                            FullName = followingUser.FullName,
                            ProfilePicturePath = followingUser.ProfilePicturePath,
                            UserName = followingUser.UserName,
                        }
            };
        }

        #endregion Methods
    }
}
