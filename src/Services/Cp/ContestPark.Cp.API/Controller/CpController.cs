using ContestPark.Domain.Cp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Cp.API.Controller
{
    public class CpController : Core.Service.ControllerBase
    {
        #region Private variables

        private readonly ICpGrain _cpGrain;

        #endregion Private variables

        #region Constructor

        public CpController(
            ILogger<CpController> logger,
            IClusterClient clusterClient
        ) : base(logger)
        {
            _cpGrain = clusterClient.GetGrain<ICpGrain>(0);
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Giriş yapan kullanıcnın toplam altın miktarını verir
        /// </summary>
        /// <returns>Toplam altın miktarı</returns>
        [HttpGet]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            int gold = await _cpGrain.GetTotalGoldByUserId(UserId);

            return Ok(gold);
        }

        #endregion Methods
    }
}