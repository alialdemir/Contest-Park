using ContestPark.Core.Database.Models;
using ContestPark.Mission.API.Infrastructure.Repositories.CompletedMission;
using ContestPark.Mission.API.Infrastructure.Repositories.Mission;
using ContestPark.Mission.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace ContestPark.Mission.API.Controllers
{
    public class MissionController : Core.Controllers.ControllerBase
    {
        #region Private variables

        private readonly ICompletedMissionRepository _completedMissionRepository;
        private readonly IMissionRepository _missionRepository;

        #endregion Private variables

        #region Constructor

        public MissionController(ILogger<MissionController> logger,
                                 ICompletedMissionRepository completedMissionRepository,
                                 IMissionRepository missionRepository) : base(logger)
        {
            _completedMissionRepository = completedMissionRepository;
            _missionRepository = missionRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Görev listesi
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Görevler</returns>
        [HttpGet]
        [ProducesResponseType(typeof(MissionServiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Missions([FromQuery] PagingModel pagingModel)
        {
            var missions = _missionRepository.GetMissions(UserId, CurrentUserLanguage, pagingModel);
            if (missions == null || !missions.Items.Any())
                return NotFound();

            return Ok(new MissionServiceModel
            {
                HasNextPage = missions.HasNextPage,
                Items = missions.Items,
                PageNumber = missions.PageNumber,
                PageSize = missions.PageSize,
                CompletedMissionCount = _completedMissionRepository.CompletedMissionCount(UserId)
            });
        }

        /// <summary>
        /// Tamamlanan görevin altınlarını verir
        /// </summary>
        /// <param name="missionId">Görev id</param>
        /// <returns>Görev altınları yüklenmiş ise true yüklenmemiş ise false</returns>
        [HttpPost("{missionId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult TakesMissionReward([FromRoute] byte missionId)
        {
            bool isMissionCompleted = _completedMissionRepository.TakesMissionReward(UserId, missionId);
            if (!isMissionCompleted)
                return BadRequest();

            return Ok();
        }

        #endregion Methods
    }
}
