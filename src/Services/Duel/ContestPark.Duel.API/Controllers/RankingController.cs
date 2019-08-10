using ContestPark.Core.Database.Models;
using ContestPark.Core.Models;
using ContestPark.Core.Services.Identity;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.ContestDate;
using ContestPark.Duel.API.Infrastructure.Repositories.ScoreRankingRepository;
using ContestPark.Duel.API.Models;
using ContestPark.Duel.API.Services.Follow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Controllers
{
    public class RankingController : ContestPark.Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IScoreRankingRepository _scoreRankingRepository;
        private readonly IContestDateRepository _contestDateRepository;
        private readonly IIdentityService _identityService;
        private readonly IFollowService _followService;

        #endregion Private Variables

        #region Constructor

        public RankingController(IScoreRankingRepository scoreRankingRepository,
                                 IContestDateRepository contestDateRepository,
                                 IIdentityService identityService,
                                 IFollowService followService,
                                 ILogger<RankingController> logger) : base(logger)
        {
            _scoreRankingRepository = scoreRankingRepository;
            _contestDateRepository = contestDateRepository;
            _identityService = identityService;
            _followService = followService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Bakiye tipine göre alt kategori skor sıralaması getir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="balanceType">Bakiye tipi</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Sıralama ve yarışma bitiş tarihi</returns>
        [HttpGet("SubCategory/{subCategoryId}")]
        [ProducesResponseType(typeof(RankingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get([FromRoute]short subCategoryId,
                                 [EnumDataType(typeof(BalanceTypes), ErrorMessage = "Balance type value doesn't exist within enum")]
                                 [FromQuery]BalanceTypes balanceType,
                                 [FromQuery]PagingModel pagingModel)
        {
            if (subCategoryId <= 0)
                return BadRequest();

            ContestDateModel contestDate = _contestDateRepository.ActiveContestDate();
            if (contestDate == null)
                return NotFound();

            RankingModel result = new RankingModel
            {
                ContestFinishDate = contestDate.FinishDate,
                Ranks = _scoreRankingRepository.GetRankingBySubCategoryId(subCategoryId, balanceType, contestDate.ContestDateId, pagingModel)
            };
            if (result.Ranks == null || result.Ranks.Items.Count() == 0)
                return Ok(result);

            return Ok(await GetRankingModel(result));
        }

        [HttpGet("SubCategory/{subCategoryId}/Following")]
        [ProducesResponseType(typeof(RankingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetFollowingRank([FromRoute]short subCategoryId,
                                 [EnumDataType(typeof(BalanceTypes), ErrorMessage = "Balance type value doesn't exist within enum")]
                                 [FromQuery]BalanceTypes balanceType,
                                 [FromQuery]PagingModel pagingModel)
        {
            if (subCategoryId <= 0)
                return BadRequest();

            ContestDateModel contestDate = _contestDateRepository.ActiveContestDate();
            if (contestDate == null)
                return NotFound();

            IEnumerable<string> followingUsers = await _followService.GetFollowingUserIds(UserId, pagingModel);
            if (followingUsers == null)
                return NotFound();

            RankingModel result = new RankingModel
            {
                ContestFinishDate = contestDate.FinishDate,
                Ranks = _scoreRankingRepository.GetFollowingRanking(subCategoryId, balanceType, contestDate.ContestDateId, followingUsers, pagingModel)
            };
            if (result.Ranks == null || result.Ranks.Items.Count() == 0)
                return Ok(result);

            return Ok(await GetRankingModel(result));
        }

        /// <summary>
        /// Sıralama lsitesindeki kullanıcı bilgilerini identity servisten alıp modele ekleyip döndürür
        /// </summary>
        /// <param name="result">Sıralama Listesi</param>
        /// <returns>Sıralama sonucu</returns>
        private async Task<RankingModel> GetRankingModel(RankingModel result)
        {
            IEnumerable<string> userIds = result.Ranks.Items.Select(r => r.UserId).AsEnumerable();

            IEnumerable<UserModel> users = await _identityService.GetUserInfosAsync(userIds);
            if (users != null && users.Count() != 0)
            {
                result.Ranks.Items.ToList().ForEach(r =>
                {
                    UserModel user = users.FirstOrDefault(u => u.UserId == r.UserId);
                    r.UserFullName = user.FullName;
                    r.UserName = user.UserName;
                    r.UserProfilePicturePath = user.ProfilePicturePath;
                });
            }

            return result;
        }

        #endregion Methods
    }
}
