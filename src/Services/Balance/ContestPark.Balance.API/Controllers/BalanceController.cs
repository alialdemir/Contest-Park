using ContestPark.Balance.API.Enums;
using ContestPark.Balance.API.Infrastructure.Documents;
using ContestPark.Balance.API.Infrastructure.Repositories.Balance;
using ContestPark.Balance.API.Infrastructure.Repositories.Purchase;
using ContestPark.Balance.API.Models;
using ContestPark.Balance.API.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Controllers
{
    public class BalanceController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IBalanceRepository _balanceRepository;
        private readonly IPurchaseHistoryRepository _purchaseHistoryRepository;

        #endregion Private Variables

        #region Constructor

        public BalanceController(IBalanceRepository balanceRepository,
                                 ILogger<BalanceController> logger,
                                 IPurchaseHistoryRepository purchaseHistoryRepository) : base(logger)
        {
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _purchaseHistoryRepository = purchaseHistoryRepository ?? throw new ArgumentNullException(nameof(purchaseHistoryRepository));
        }

        #endregion Constructor

        #region Properties

        private Dictionary<string, decimal> _googlePlayPackages;

        /// <summary>
        /// Google playde tanımlı paket isimleri
        /// </summary>
        public Dictionary<string, decimal> GooglePlayPackages
        {
            get
            {
                if (_googlePlayPackages == null)
                {
                    _googlePlayPackages = new Dictionary<string, decimal>
                    {
                        {"com.contestparkapp.app.250coins", 250 },
                        {"com.contestparkapp.app.1500coins", 1500 },
                        {"com.contestparkapp.app.7000coins", 7000 },
                        {"com.contestparkapp.app.20000coins", 20000},
                    };
                }
                return _googlePlayPackages;
            }
        }

        //private Dictionary<string, decimal> _appStorePackages;

        ///// <summary>
        ///// App store tanımlı paket isimleri
        ///// </summary>
        //public Dictionary<string, decimal> AppStorePackages
        //{
        //    get
        //    {
        //        if (_appStorePackages == null)
        //        {
        //            _appStorePackages = new Dictionary<string, decimal>
        //            {
        //                {"com.contestparkapp.app.250coins", 250 },
        //                {"com.contestparkapp.app.1500coins",1500 },
        //                {"com.contestparkapp.app.7000coins",7000 },
        //                {"com.contestparkapp.app.20000coins", 20000},
        //            };
        //        }
        //        return _appStorePackages;
        //    }
        //}

        #endregion Properties

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
            IEnumerable<BalanceModel> result = _balanceRepository.GetUserBalances(UserId);
            if (result == null || result.Count() == 0)
            {
                Logger.LogInformation($"Kullanıcının bakiye bilgilerine ulaşılamadı.", UserId);

                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// User id göre bakiye bilgilerinin getirir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Bakiye bilgileri</returns>
        [HttpGet("{userId}")]
        [Authorize(Policy = "ContestParkServices")]
        [ProducesResponseType(typeof(BalanceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
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

            var result = balances
                            .Where(b => b.BalanceType == balanceType)
                            .Select(b => new BalanceModel
                            {
                                Amount = b.Amount
                            })
                            .FirstOrDefault();

            return Ok(result);
        }

        /// <summary>
        /// Bakiye satın aldığında hesaba yükleme işlemi yapar
        /// </summary>
        /// <param name="purchase">Satın alma bilgileri</param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Purchase([FromBody]PurchaseModel purchase)
        {
            #region Validations

            decimal amount = GetAmountByPackageName(purchase.PackageName);
            if (amount == 0)
                return BadRequest(BalanceResource.PackagenameIsIncorrect);

            #endregion Validations

            Logger.LogInformation("Paket yükleme isteği geldi...", purchase.PackageName, purchase.ProductId);

            // TODO: google play istek atılıp token ve packet adı ile satın alım başarılı olmuşmu kontrol edilmeli

            BalanceTypes? balanceType = GetBalanceTypeByPackageName(purchase.PackageName);

            bool isSuccess = await _balanceRepository.UpdateBalanceAsync(new ChangeBalanceModel
            {
                UserId = UserId,
                BalanceHistoryType = BalanceHistoryTypes.Buy,
                BalanceType = balanceType ?? BalanceTypes.Gold,// gold hiçbir zaman gelmemesi lazım yukaruda null kontrol var
                Amount = amount
            });

            if (!isSuccess)
                return BadRequest(BalanceResource.ThePurchaseFailedPleaseEmailWithOurSupportTeam);

            AddPurchaseHistory(purchase, amount, balanceType ?? BalanceTypes.Gold);

            return Ok();
        }

        /// <summary>
        /// Satın alma geçmişi ekler
        /// </summary>
        /// <param name="purchase">Satın alma bilgileri</param>
        /// <param name="amount">Satın alınan altın</param>
        /// <param name="balanceType">Satın alma itpi</param>
        private void AddPurchaseHistory(PurchaseModel purchase, decimal amount, BalanceTypes balanceType)
        {
            _purchaseHistoryRepository.AddAsync(new PurchaseHistory
            {
                Amount = amount,
                UserId = UserId,
                BalanceType = balanceType,// gold hiçbir zaman gelmemesi lazım yukaruda null kontrol var
                ProductId = purchase.ProductId,
                PackageName = purchase.PackageName,
                Token = purchase.Token,
                Platform = purchase.Platform
            }).Wait();
        }

        /// <summary>
        /// Paket isimlerine göre bakiye tipini verir
        /// </summary>
        /// <param name="packageName">Google play/App store paket name</param>
        /// <returns>Bakiye tipi</returns>
        private BalanceTypes? GetBalanceTypeByPackageName(string packageName)
        {
            // TODO: app store paketlerine göre balance type gelmeli

            if (GooglePlayPackages.ContainsKey(packageName))
            {
                return BalanceTypes.Gold;
            }

            return null;
        }

        /// <summary>
        /// Paket isimlerine göre ne kadarlık bakiye yüklenceğini verir
        /// </summary>
        /// <param name="packageName">Google play/App store paket name</param>
        /// <returns>Yüklenecek bakiye</returns>
        private decimal GetAmountByPackageName(string packageName)
        {
            // TODO: app store paketleri tanımlanmalı

            if (GooglePlayPackages.ContainsKey(packageName))
            {
                return GooglePlayPackages[packageName];
            }

            return 0;
        }

        #endregion Methods
    }
}
