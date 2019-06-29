﻿using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.EventBus.Abstractions;
using ContestPark.Follow.API.Infrastructure.Documents;
using ContestPark.Follow.API.Infrastructure.Repositories.Follow;
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
        private readonly IRepository<User> _userRepository;
        private readonly IEventBus _eventBus;

        #endregion Private Variables

        #region Constructor

        public FollowController(IFollowRepository followRepository,
                                IRepository<User> userRepository,
                                IEventBus eventBus,
                                ILogger<FollowController> logger) : base(logger)
        {
            _followRepository = followRepository ?? throw new ArgumentNullException(nameof(followRepository));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
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
        public async Task<IActionResult> Post([FromRoute]string followedUserId)
        {
            if (string.IsNullOrEmpty(followedUserId) || UserId == followedUserId)
                return BadRequest();

            if (_followRepository.CheckFollowUpStatus(UserId, followedUserId))
                return BadRequest(FollowResource.YouAreAlreadyFollowingThisUser);

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
        public async Task<IActionResult> Delete([FromRoute]string followedUserId)
        {
            if (string.IsNullOrEmpty(followedUserId) || UserId == followedUserId)
                return BadRequest();

            if (!_followRepository.CheckFollowUpStatus(UserId, followedUserId))
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
        [ProducesResponseType(typeof(ServiceModel<FollowModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetFollowing(string userId, [FromQuery]PagingModel pagingModel)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            ServiceModel<string> following = _followRepository.Following(userId, pagingModel);
            if (following.Items.Count() == 0)
                return NotFound();

            IEnumerable<string> currentUserFollowers = _followRepository.CheckFollowUpStatus(UserId, following.Items);
            IEnumerable<User> followingUsers = _userRepository.FindByIds(following.Items);

            return Ok(GetFollowServiceModel(following, currentUserFollowers, followingUsers, pagingModel));
        }

        /// <summary>
        /// Parametreden gelen kullanıcının takip edenler
        /// </summary>
        /// <returns>Başarılı ise OK değilse BadRequest mesajı döndürür.</returns>
        [HttpGet("{userId}/Followers")]
        [ProducesResponseType(typeof(ServiceModel<FollowModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetFollowers(string userId, [FromQuery]PagingModel pagingModel)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            ServiceModel<string> followers = _followRepository.Followers(userId, pagingModel);
            if (followers.Items.Count() == 0)
                return NotFound();

            IEnumerable<string> currentUserFollowers = _followRepository.CheckFollowUpStatus(UserId, followers.Items);
            IEnumerable<User> followingUsers = _userRepository.FindByIds(followers.Items);

            return Ok(GetFollowServiceModel(followers, currentUserFollowers, followingUsers, pagingModel));
        }

        private ServiceModel<FollowModel> GetFollowServiceModel(ServiceModel<string> followers,
                                                                IEnumerable<string> currentUserFollowers,
                                                                IEnumerable<User> followingUsers,
                                                                PagingModel pagingModel)
        {
            Task.Factory.StartNew(() =>
            {
                // User tablosunda olmayan kullanıcıları identity den istemek için event publish ettik
                var notFoundUserIds = followers.Items.Where(u => !followingUsers.Any(x => x.Id == u)).AsEnumerable();
                if (notFoundUserIds.Count() > 0)
                {
                    var @event = new UserNotFoundIntegrationEvent(notFoundUserIds);
                    _eventBus.Publish(@event);
                }
            });
            return new ServiceModel<FollowModel>
            {
                HasNextPage = followers.HasNextPage,
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize,
                Items = from follower in followers.Items
                        join followingUser in followingUsers on follower equals followingUser.Id
                        join currentUserFollower in currentUserFollowers on follower equals currentUserFollower into currentUserFollowerData
                        from p in currentUserFollowerData.DefaultIfEmpty()
                        select new FollowModel
                        {
                            UserId = follower,
                            IsFollowing = currentUserFollowerData.Any(),
                            FullName = followingUser.FullName,
                            ProfilePicturePath = followingUser.ProfilePicturePath,
                            UserName = followingUser.UserName,
                        }
            };
        }

        #endregion Methods
    }
}