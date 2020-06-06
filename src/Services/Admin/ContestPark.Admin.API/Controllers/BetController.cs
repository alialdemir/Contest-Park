using ContestPark.Admin.API.Infrastructure.Repositories.Bet;
using ContestPark.Admin.API.Model.Bet;
using ContestPark.Core.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class BetController : Core.Controllers.ControllerBase
    {
        #region Private variables

        private readonly IBetRepository _betRepository;

        #endregion Private variables

        #region Constructor

        public BetController(ILogger<BetController> logger,
                             IBetRepository betRepository) : base(logger)
        {
            _betRepository = betRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Bahis listesini döndürür
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Sayfalama yapılmış bahis listesi</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceModel<BetModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetBetList([FromQuery] PagingModel pagingModel)
        {
            ServiceModel<BetModel> bets = _betRepository.GetBetList(pagingModel);
            if (bets == null || !bets.Items.Any())
                return NotFound();

            return Ok(bets);
        }

        /// <summary>
        /// Bahis güncelle
        /// </summary>
        /// <param name="betUpdateModel">Güncellenen bahis bilgisi</param>
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody]BetUpdateModel betUpdateModel)
        {
            if (betUpdateModel == null
                || betUpdateModel.BetId <= 0
                || string.IsNullOrEmpty(betUpdateModel.Title)
                || string.IsNullOrEmpty(betUpdateModel.Description)
                || betUpdateModel.EarnedCoin < 0
                || betUpdateModel.EntryFee < 0)
                return BadRequest();

            bool isSuccess = await _betRepository.UpdateAsync(betUpdateModel);
            if (!isSuccess)
            {
                Logger.LogError("Bahis güncelleme işlemi başarısız oldu. Bet id: {betId}", betUpdateModel.BetId);
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// Bahis ekle
        /// </summary>
        /// <param name="betAddModel">eklenen bahis bilgisi</param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddAsync([FromBody]BetAddModel betAddModel)
        {
            if (betAddModel == null
                || string.IsNullOrEmpty(betAddModel.Title)
                || string.IsNullOrEmpty(betAddModel.Description)
                || betAddModel.EarnedCoin < 0
                || betAddModel.EntryFee < 0)
                return BadRequest();

            bool isSuccess = await _betRepository.AddAsync(new Infrastructure.Tables.Bet
            {
                BalanceType = betAddModel.BalanceType,
                Description = betAddModel.Description,
                EarnedCoin = betAddModel.EarnedCoin,
                EntryFee = betAddModel.EntryFee,
                Title = betAddModel.Title
            });
            if (!isSuccess)
            {
                Logger.LogError("Bahis ekleme işlemi başarısız oldu.");
                return BadRequest();
            }

            return Ok();
        }

        // restfull eklemede bu şekilde silmeyi yapmak ister misin????* melih ordamsn  tamam geldi
        // yazabilir misin silmeyi?deneyelim kanka silme methodu neydi remove assyn deil mi evet ama şuan sen controller methodunu yazıon
        /// <summary>
        /// Bahis sil
        /// </summary>
        /// <param name="betId">Bahis id</param>
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveAsync([FromRoute]byte betId)
        {
            if (betId < 0)
                return BadRequest();

            bool isSuccess = await _betRepository.ClearAsync(betId);
            if (!isSuccess)
            {
                Logger.LogError("Bahis silme işlemi başarısız oldu. Silinmeye çalışılan bahis id: {betId}", betId);// loglara bakınca hatayı rahat anlayalım diye -tamm

                return BadRequest();
            }

            return Ok();
        }

        #endregion Methods
    }
}
