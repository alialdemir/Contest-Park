﻿using ContestPark.Balance.API.Enums;
using ContestPark.Balance.API.Infrastructure.Repositories.Balance;
using ContestPark.Balance.API.Infrastructure.Repositories.MoneyWithdrawRequest;
using ContestPark.Balance.API.Infrastructure.Repositories.PurchaseHistory;
using ContestPark.Balance.API.Infrastructure.Repositories.Reference;
using ContestPark.Balance.API.Infrastructure.Repositories.ReferenceCode;
using ContestPark.Balance.API.Infrastructure.Tables;
using ContestPark.Balance.API.IntegrationEvents.Events;
using ContestPark.Balance.API.Models;
using ContestPark.Balance.API.Resources;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Controllers
{
    public class BalanceController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IBalanceRepository _balanceRepository;
        private readonly IReferenceRepository _referenceRepository;
        private readonly IReferenceCodeRepostory _referenceCodeRepostory;
        private readonly IEventBus _eventBus;
        private readonly IMoneyWithdrawRequestRepository _moneyWithdrawRequestRepository;
        private readonly IPurchaseHistoryRepository _purchaseHistoryRepository;

        #endregion Private Variables

        #region Constructor

        public BalanceController(IBalanceRepository balanceRepository,
                                 IReferenceRepository referenceRepository,
                                 IReferenceCodeRepostory referenceCodeRepostory,
                                 IEventBus eventBus,
                                 IMoneyWithdrawRequestRepository moneyWithdrawRequestRepository,
                                 ILogger<BalanceController> logger,
                                 IPurchaseHistoryRepository purchaseHistoryRepository) : base(logger)
        {
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _referenceRepository = referenceRepository;
            _referenceCodeRepostory = referenceCodeRepostory;
            _eventBus = eventBus;
            _moneyWithdrawRequestRepository = moneyWithdrawRequestRepository;
            _purchaseHistoryRepository = purchaseHistoryRepository ?? throw new ArgumentNullException(nameof(purchaseHistoryRepository));
        }

        #endregion Constructor

        #region Properties

        private Dictionary<string, PackageModel> _googlePlayPackages;

        /// <summary>
        /// Google playde tanımlı paket isimleri
        /// </summary>
        public Dictionary<string, PackageModel> GooglePlayPackages
        {
            get
            {
                if (_googlePlayPackages == null)
                {
                    _googlePlayPackages = new Dictionary<string, PackageModel>
                    {
                        // Gold
                        {"com.contestparkapp.app.250coins", new PackageModel{ Amount=2.50m, BalanceType= BalanceTypes.Gold } },
                        {"com.contestparkapp.app.1500coins",  new PackageModel{ Amount=15.00m, BalanceType= BalanceTypes.Gold } },
                        {"com.contestparkapp.app.7000coins",  new PackageModel{ Amount=7.000m, BalanceType= BalanceTypes.Gold } },
                        {"com.contestparkapp.app.20000coins",  new PackageModel{ Amount=20.000m , BalanceType= BalanceTypes.Gold }},
                        // Money
                        {"com.contestparkapp.app.1dolar",  new PackageModel{ Amount=1.00m , BalanceType= BalanceTypes.Money }},
                        {"com.contestparkapp.app.4dolar",  new PackageModel{ Amount=4.00m , BalanceType= BalanceTypes.Money }},
                        {"com.contestparkapp.app.9dolar",  new PackageModel{ Amount=9.00m , BalanceType= BalanceTypes.Money }},
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
        /// Bakiye koduna göre hesaba bakiye yükler
        /// </summary>
        /// <param name="balanceCode">Bakiye kodu</param>
        [HttpPost("Code")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BalanceCode([FromBody]BalanceCodeModel balanceCode)
        {
            if (balanceCode == null || string.IsNullOrEmpty(balanceCode.Code))
                return BadRequest();

            ReferenceModel reference = _referenceRepository.IsCodeActive(balanceCode.Code, UserId);
            if (reference == null)
                return NotFound();

            bool isSuccess = await _referenceCodeRepostory.Insert(balanceCode.Code, string.Empty, UserId);
            if (!isSuccess)
            {
                Logger.LogError("Bakiye kodu ile bakiye yükleme işlemi sırasında hata oluştu. {code} {UserId}", balanceCode.Code, UserId);

                return BadRequest();
            }

            var @event = new ChangeBalanceIntegrationEvent(reference.Amount, UserId, reference.BalanceType, BalanceHistoryTypes.ReferenceCode);

            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Giriş yapan kullanıcının tüm bakiye bilgilerini döner
        /// </summary>
        /// <returns>Bakiye bilgileri</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BalanceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetBalances()
        {
            BalanceModel result = _balanceRepository.GetUserBalances(UserId);
            if (result == null)
            {
                Logger.LogInformation($"Kullanıcının bakiye bilgilerine ulaşılamadı.", UserId);

                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Iban numarasına para gönderme isteği
        /// </summary>
        /// <param name="ibanNoModel">Iban no ve ad soyad</param>
        /// <returns>Başarılı ise ok değilse hata mesajı</returns>
        [HttpPost()]
        [ProducesResponseType(typeof(IbanNoModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SendMoneyRequest([FromBody]IbanNoModel ibanNoModel)
        {
            Logger.LogInformation($"Para çekme isteği geldi. Ad soyad: {ibanNoModel.FullName} user Id: {UserId} Iban No: {ibanNoModel.IbanNo}");

            if (ibanNoModel == null || string.IsNullOrEmpty(ibanNoModel.FullName) || string.IsNullOrEmpty(ibanNoModel.IbanNo))
            {
                return BadRequest();
            }

            BalanceModel result = _balanceRepository.GetUserBalances(UserId);
            if (result == null || result.Money < 100.00m)
            {
                Logger.LogInformation($"Yetersiz bakiye ile para çekme talebi geldi.", UserId);

                return NotFound();
            }

            Logger.LogInformation($"Para çekme isteği şuanki para miktarı. User Id: {UserId} Money: {result.Money}");

            bool isSuccess = await SendMoneyRequestUpdateBalanceAsync(-result.Money);// Önce hesaptan para düşüldü
            if (!isSuccess)
            {
                return BadRequest(BalanceResource.TheWithdrawalRequestFailed);
            }

            isSuccess = await _moneyWithdrawRequestRepository.Insert(new MoneyWithdrawRequest// Para çekme isteği geldiğine dair kayıt ekledik
            {
                UserId = UserId,
                FullName = ibanNoModel.FullName,
                IbanNo = ibanNoModel.IbanNo,
                Amount = result.Money,
                Status = Status.Active
            });

            if (!isSuccess)
            {
                Logger.LogCritical($@"CRITICAL: Bakiye çekme isteği sırasında hata oluştu. Ad soyad: {ibanNoModel.FullName}
                                                                                           User Id: {UserId}
                                                                                           Iban No: {ibanNoModel.IbanNo}");

                await SendMoneyRequestUpdateBalanceAsync(result.Money);// hata oluşursa bakiye geri hesaba eklendi

                return BadRequest(BalanceResource.TheWithdrawalRequestFailed);
            }

            // TODO: bizim tarafa mail gönder veya bir alert atılsın

            Logger.LogInformation($"Para çekme isteği oluşturuldu. User Id: {UserId} Money: {-result.Money}");

            return Ok();
        }

        /// <summary>
        /// Para çekme isteği oluşturan kullanıcının hesabındaki parayı günceller
        /// </summary>
        /// <param name="money">Eksiltilecek para birimi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        private Task<bool> SendMoneyRequestUpdateBalanceAsync(decimal money)
        {
            return _balanceRepository.UpdateBalanceAsync(new ChangeBalanceModel
            {
                UserId = UserId,
                BalanceHistoryType = BalanceHistoryTypes.MoneyWithdraw,
                BalanceType = BalanceTypes.Money,
                Amount = money
            });
        }

        /// <summary>
        /// User id göre bakiye bilgilerinin getirir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Bakiye bilgileri</returns>
        [HttpGet("{userId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BalanceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetBalances(string userId, [FromQuery]BalanceTypes balanceType)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            BalanceModel balance = _balanceRepository.GetUserBalances(userId);
            if (balance == null)
            {
                Logger.LogInformation($"Kullanıcının bakiye bilgilerine ulaşılamadı.", userId);

                return NotFound();
            }

            decimal amount = 0;

            switch (balanceType)
            {
                case BalanceTypes.Gold:
                    amount = balance.Gold;
                    break;

                case BalanceTypes.Money:
                    amount = balance.Money;
                    break;
            }

            return Ok(new { Amount = amount });
        }

        /// <summary>
        /// Bakiye satın aldığında hesaba yükleme işlemi yapar
        /// </summary>
        /// <param name="purchase">Satın alma bilgileri</param>
        [HttpPost("Purchase")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Purchase([FromBody]PurchaseModel purchase)
        {
            Logger.LogError($@"Paket yükleme isteği geldi... UserId: {UserId}
                                                             PackageName: {purchase.PackageName}
                                                             Platform: {purchase.Platform}
                                                             ProductId: {purchase.ProductId}
                                                             Token: {purchase.Token}");

            #region Validations

            PackageModel package = GetPackageByPackageName(purchase.PackageName);
            if (package == null || package.Amount == 0)
            {
                Logger.LogError($@"Uygulama içi satın alma hata!. Paket adından bakiye tipi alınırken null değer geldi. Acil kontrol ediniz. UserId: {UserId}
                                                                                                              PackageName: {purchase.PackageName}
                                                                                                              Platform: {purchase.Platform}
                                                                                                              ProductId: {purchase.ProductId}
                                                                                                              Token: {purchase.Token}");
                return BadRequest(BalanceResource.PackagenameIsIncorrect);
            }

            #endregion Validations

            // TODO: google play istek atılıp token ve packet adı ile satın alım başarılı olmuşmu kontrol edilmeli

            bool isSuccess = await _balanceRepository.UpdateBalanceAsync(new ChangeBalanceModel
            {
                UserId = UserId,
                BalanceHistoryType = BalanceHistoryTypes.Buy,
                BalanceType = package.BalanceType,// gold hiçbir zaman gelmemesi lazım yukaruda null kontrol var
                Amount = package.Amount
            });

            if (!isSuccess)
                return BadRequest(BalanceResource.ThePurchaseFailedPleaseEmailWithOurSupportTeam);

            await AddPurchaseHistory(purchase, package);

            return Ok();
        }

        /// <summary>
        /// Satın alma geçmişi ekler
        /// </summary>
        /// <param name="purchase">Satın alma bilgileri</param>
        /// <param name="amount">Satın alınan altın</param>
        /// <param name="balanceType">Satın alma itpi</param>
        private async Task AddPurchaseHistory(PurchaseModel purchase, PackageModel package)
        {
            await _purchaseHistoryRepository.AddAsync(new PurchaseHistory
            {
                Amount = package.Amount,
                UserId = UserId,
                BalanceType = package.BalanceType,
                ProductId = purchase.ProductId,
                PackageName = purchase.PackageName,
                Token = purchase.Token,
                Platform = purchase.Platform
            });
        }

        /// <summary>
        /// Paket isimlerine göre paket bilgilerini verir
        /// </summary>
        /// <param name="packageName">Google play/App store paket name</param>
        /// <returns>Bakiye tipi</returns>
        private PackageModel GetPackageByPackageName(string packageName)
        {
            // TODO: app store paketlerine göre balance type gelmeli

            if (GooglePlayPackages.ContainsKey(packageName))
            {
                return GooglePlayPackages[packageName];
            }

            return null;
        }

        #endregion Methods
    }
}
