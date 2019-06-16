using ContestPark.Balance.API.Infrastructure.Repositories.Cp;
using ContestPark.Balance.API.Models;
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
        /// Kullanıcının tüm bakiye bilgilerini döner
        /// </summary>
        /// <returns>Bakiye bilgileri</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BalanceModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetBalances()
        {
            IEnumerable<BalanceModel> balances = _balanceRepository.GetUserBalances(UserId);
            if (balances == null || balances.Count() == 0)
                return NotFound();

            return Ok(balances);
        }

        #endregion Methods
    }
}