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

        private readonly ICpGrain _categoryGrain;

        #endregion Private variables

        #region Constructor

        public CpController(
            ILogger<CpController> logger,
            IClusterClient clusterClient
        ) : base(logger)
        {
            _categoryGrain = clusterClient.GetGrain<ICpGrain>(0);
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
            return Ok(await _categoryGrain.GetTotalGoldByUserId(UserId));
        }

        #endregion Methods
    }
}