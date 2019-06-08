using ContestPark.Follow.API.Infrastructure.Repositories.Follow;
using ContestPark.Follow.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Follow.API.Controllers
{
    public class FollowController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IFollowRepository _followRepository;

        #endregion Private Variables

        #region Constructor

        public FollowController(IFollowRepository followRepository,
                                ILogger<FollowController> logger) : base(logger)
        {
            _followRepository = followRepository ?? throw new ArgumentNullException(nameof(followRepository));
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

        #endregion Methods
    }
}