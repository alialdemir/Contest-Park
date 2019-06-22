using ContestPark.Balance.API.Enums;
using ContestPark.Balance.API.Infrastructure.Repositories.Balance;
using ContestPark.Balance.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ContestPark.Balance.API.Controllers
{
    public class BalanceController : ContestPark.Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IBalanceRepository _balanceRepository;

        #endregion Private Variables

        #region Constructor

        public BalanceController(IBalanceRepository balanceRepository,
                                 ILogger<BalanceController> logger) : base(logger)
        {
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Giriş yapan kullanıcının tüm bakiye bilgilerini döner
        /// </summary>
        /// <returns>Bakiye bilgileri</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BalanceModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetBalances()
        {
            IEnumerable<BalanceModel> balances = _balanceRepository.GetUserBalances(UserId);
            if (balances == null || balances.Count() == 0)
            {
                Logger.LogInformation($"Kullanıcının bakiye bilgilerine ulaşılamadı.", UserId);

                return NotFound();
            }

            return Ok(balances);
        }

        /// <summary>
        /// User id göre bakiye bilgilerinin getirir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Bakiye bilgileri</returns>
        [HttpGet("{userId}")]
        [Authorize(Policy = "ContestParkServices")]
        [ProducesResponseType(typeof(BalanceModel), (int)HttpStatusCode.OK)]
        public IActionResult GetBalances(string userId, [FromQuery]BalanceTypes balanceType = BalanceTypes.Gold)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            IEnumerable<BalanceModel> balances = _balanceRepository.GetUserBalances(userId);
            if (balances == null || balances.Count() == 0)
            {
                Logger.LogInformation($"Kullanıcının bakiye bilgilerine ulaşılamadı.", userId);

                return NotFound();
            }

            var balance = balances
                            .Where(b => b.BalanceType == balanceType)
                            .Select(b => new BalanceModel
                            {
                                Amount = b.Amount
                            })
                            .FirstOrDefault();

            return Ok(balance);
        }

        #endregion Methods
    }
}