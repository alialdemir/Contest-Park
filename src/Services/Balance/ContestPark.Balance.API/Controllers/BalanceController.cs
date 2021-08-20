﻿using ContestPark.Balance.API.Enums;
using ContestPark.Balance.API.Infrastructure.Repositories.Balance;
using ContestPark.Balance.API.Infrastructure.Repositories.BalanceHistory;
using ContestPark.Balance.API.Infrastructure.Repositories.PurchaseHistory;
using ContestPark.Balance.API.Infrastructure.Repositories.Reference;
using ContestPark.Balance.API.Infrastructure.Repositories.ReferenceCode;
using ContestPark.Balance.API.Infrastructure.Tables;
using ContestPark.Balance.API.IntegrationEvents.Events;
using ContestPark.Balance.API.Models;
using ContestPark.Balance.API.Resources;
using ContestPark.Balance.API.Services.VerifyReceiptIos;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Controllers
{
    public class BalanceController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IBalanceRepository _balanceRepository;
        private readonly IReferenceRepository _referenceRepository;
        private readonly IBalanceHistoryRepository _balanceHistoryRepository;
        private readonly IReferenceCodeRepostory _referenceCodeRepostory;
        private readonly IEventBus _eventBus;
        private readonly IVerifyReceiptIos _verifyReceiptIos;
        private readonly IPurchaseHistoryRepository _purchaseHistoryRepository;

        #endregion Private Variables

        #region Constructor

        public BalanceController(IBalanceRepository balanceRepository,
                                 IReferenceRepository referenceRepository,
                                 IBalanceHistoryRepository balanceHistoryRepository,
                                 IReferenceCodeRepostory referenceCodeRepostory,
                                 IEventBus eventBus,
                                 IVerifyReceiptIos verifyReceiptIos,
                                 ILogger<BalanceController> logger,
                                 IPurchaseHistoryRepository purchaseHistoryRepository) : base(logger)
        {
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _referenceRepository = referenceRepository;
            _balanceHistoryRepository = balanceHistoryRepository;
            _referenceCodeRepostory = referenceCodeRepostory;
            _eventBus = eventBus;
            _verifyReceiptIos = verifyReceiptIos;
            _purchaseHistoryRepository = purchaseHistoryRepository ?? throw new ArgumentNullException(nameof(purchaseHistoryRepository));
        }

        #endregion Constructor

        #region Properties

        private Dictionary<string, PackageModel> _googlePlayPackages;

        /// <summary>
        /// Google play tanımlı paket isimleri
        /// </summary>
        public Dictionary<string, PackageModel> Products
        {
            get
            {
                if (_googlePlayPackages == null)
                {
                    _googlePlayPackages = new Dictionary<string, PackageModel>
                    {
                        // Android

                        // Gold
                        {"com.contestparkapp.app.250coins", new PackageModel{ Amount = 2.50m, BalanceType= BalanceTypes.Gold } },
                        {"com.contestparkapp.app.1500coins",  new PackageModel{ Amount = 15.00m, BalanceType= BalanceTypes.Gold } },
                        {"com.contestparkapp.app.7000coins",  new PackageModel{ Amount = 7.00m, BalanceType= BalanceTypes.Gold } },
                        {"com.contestparkapp.app.20000coins",  new PackageModel{ Amount = 20.000m , BalanceType= BalanceTypes.Gold }},
                        // Money
                     //   {"com.contestparkapp.app.6",  new PackageModel{ Amount = 6.99m , BalanceType= BalanceTypes.Money }},
                        {"com.contestparkapp.app.12",  new PackageModel{ Amount = 12.99m , BalanceType= BalanceTypes.Money }},
                        {"com.contestparkapp.app.19",  new PackageModel{ Amount = 19.99m , BalanceType= BalanceTypes.Money }},
                        {"com.contestparkapp.app.26",  new PackageModel{ Amount = 26.99m , BalanceType= BalanceTypes.Money, IsSpecialOffer = true }},

                        // Ios

                        // Gold
                        {"com.contestpark.app.250Coins", new PackageModel{ Amount = 2.50m, BalanceType= BalanceTypes.Gold } },
                        {"com.contestpark.app.1500Coins",  new PackageModel{ Amount = 15.00m, BalanceType= BalanceTypes.Gold } },
                        {"com.contestpark.app.700Coins",  new PackageModel{ Amount = 7.00m, BalanceType= BalanceTypes.Gold } },
                        {"com.contestpark.app.20000Coins",  new PackageModel{ Amount = 20.000m , BalanceType= BalanceTypes.Gold }},
                        // Money
                  //      {"com.contestpark.app.6money",  new PackageModel{ Amount = 6.99m , BalanceType= BalanceTypes.Money }},
                        {"com.contestpark.app.12money",  new PackageModel{ Amount = 12.99m , BalanceType= BalanceTypes.Money }},
                        {"com.contestpark.app.19money",  new PackageModel{ Amount = 19.99m , BalanceType= BalanceTypes.Money }},
                        {"com.contestpark.app.26money",  new PackageModel{ Amount = 26.99m , BalanceType= BalanceTypes.Money, IsSpecialOffer = true }},
                    };
                }
                return _googlePlayPackages;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Günlük altın kazanma hakkı
        /// </summary>
        [HttpPost("Reward")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult DailyReward()
        {
            if (!_balanceHistoryRepository.IsReward(UserId))
                return NotFound();

            var rndNumber = new Random().Next(60, 120);// 60-120 arası altın üretir

            decimal reward = Convert.ToDecimal(rndNumber.ToString("N2"));

            Logger.LogInformation("Kullanıcı günlük altın kazandı. {gold} - {userId}", reward, UserId);

            var @event = new ChangeBalanceIntegrationEvent(reward,
                                                           UserId,
                                                           BalanceTypes.Gold,
                                                           BalanceHistoryTypes.DailyChip);

            _eventBus.Publish(@event);

            DateTime nextRewardDate = DateTime.Now.AddHours(12);

            var @eventNotification = new SendPushNotificationIntegrationEvent(PushNotificationTypes.Reward,
                                                                              CurrentUserLanguage,
                                                                              UserId,
                                                                              nextRewardDate);

            _eventBus.Publish(@eventNotification);

            return Ok(new
            {
                Amount = reward,
            });
        }

        /// <summary>
        /// Reklam izleyerek altın kazandı
        /// </summary>
        [HttpPost("RewardedVideo")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult RewardedVideo()
        {
            decimal rewardedVideoGoldBalance = 40;

            var @event = new ChangeBalanceIntegrationEvent(rewardedVideoGoldBalance,
                                                           UserId,
                                                           BalanceTypes.Gold,
                                                           BalanceHistoryTypes.RewardedVideo);

            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Bakiye koduna göre hesaba bakiye yükler
        /// </summary>
        /// <param name="balanceCode">Bakiye kodu</param>
        [HttpPost("Code")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BalanceCode([FromBody] BalanceCodeModel balanceCode)
        {
            if (balanceCode == null || string.IsNullOrEmpty(balanceCode.Code))
                return BadRequest();

            ReferenceModel reference = _referenceRepository.IsCodeActive(balanceCode.Code, UserId);
            if (reference == null)
                return BadRequest(BalanceResource.InvalidCode);

            bool isSuccess = await _referenceCodeRepostory.Insert(balanceCode.Code, string.Empty, UserId);
            if (!isSuccess)
            {
                Logger.LogError("Bakiye kodu ile bakiye yükleme işlemi sırasında hata oluştu. {code} {UserId}", balanceCode.Code, UserId);

                return BadRequest();
            }

            var @event = new ChangeBalanceIntegrationEvent(reference.Amount,
                                                           UserId,
                                                           reference.BalanceType,
                                                           BalanceHistoryTypes.ReferenceCode);

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
        public IActionResult GetBalances(string userId, [FromQuery] BalanceTypes balanceType)
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
        [HttpPost("Purchase/v2")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PurchaseV2([FromForm] PurchaseModel purchase)
        {
            if (purchase == null)
            {
                Logger.LogInformation("Satın alma bilgileri boş geldi. {UserId}", UserId);

                return BadRequest();
            }

            #region Yeni versiyon ile token bilgisi files içerisinde geliyor

            if (purchase.Files != null && purchase.Files.Length > 0)
            {
                using (var reader = new StreamReader(purchase.Files.OpenReadStream()))
                {
                    purchase.Token = await reader.ReadToEndAsync();
                }
            }

            #endregion Yeni versiyon ile token bilgisi files içerisinde geliyor

            return await Purchase(purchase);
        }

        /// <summary>
        /// Bakiye satın aldığında hesaba yükleme işlemi yapar
        /// </summary>
        /// <param name="purchase">Satın alma bilgileri</param>
        private async Task<IActionResult> Purchase([FromBody] PurchaseModel purchase)
        {
            if (purchase == null)
            {
                Logger.LogInformation("Satın alma bilgileri boş geldi. {UserId}", UserId);

                return BadRequest();
            }

            if (string.IsNullOrEmpty(purchase.PackageName)
                || string.IsNullOrEmpty(purchase.ProductId)
                || string.IsNullOrEmpty(purchase.VerifyPurchase)
                 || !(purchase.Platform == Platforms.Android || purchase.Platform == Platforms.Ios))
            {
                Logger.LogError("Paket satın alma bilgiler boş geldi... UserId: {UserId} PackageName: {PackageName} Platform: {Platform} ProductId: {ProductId} VerifyPurchase: {VerifyPurchase} Token: {Token}",
                                UserId,
                                purchase.PackageName,
                                purchase.Platform,
                                purchase.ProductId,
                                purchase.VerifyPurchase,
                                purchase.Token);

                return BadRequest();
            }

            if (_purchaseHistoryRepository.IsExistsToken(purchase.Token))
            {
                Logger.LogWarning("Kayıtlı olan purchase token bilgisi ile tekrar satın alınma işlemi gereçekleşti... UserId: {UserId} PackageName: {PackageName} Platform: {Platform} ProductId: {ProductId} VerifyPurchase: {VerifyPurchase} Token: {Token}",
                             UserId,
                             purchase.PackageName,
                             purchase.Platform,
                             purchase.PackageName,
                             purchase.ProductId,
                             purchase.VerifyPurchase,
                             purchase.Token);

                return BadRequest();
            }

            if (purchase.Platform == Platforms.Ios)
            {
                bool isVetify = await _verifyReceiptIos.Veriftasync(purchase.Token, new ReceiptModel
                {
                    ProductId = purchase.ProductId,
                    TransactionId = purchase.TransactionId
                });
                if (!isVetify)
                {
                    Logger.LogWarning("Geçersiz ödeme bilgisi girildi... UserId: {UserId} PackageName: {PackageName} Platform: {Platform} ProductId: {ProductId} VerifyPurchase: {VerifyPurchase} Token: {Token}",
                         UserId,
                         purchase.PackageName,
                         purchase.Platform,
                         purchase.PackageName,
                         purchase.ProductId,
                         purchase.VerifyPurchase,
                         purchase.Token);

                    return BadRequest("Invalid payment information.");
                }
            }

            Logger.LogError($@"Paket yükleme isteği geldi... UserId: {UserId}
                                                             PackageName: {purchase.PackageName}
                                                             Platform: {purchase.Platform}
                                                             ProductId: {purchase.ProductId}
                                                             VerifyPurchase: {purchase.VerifyPurchase}
                                                             Token: {purchase.Token}");

            #region Validations

            PackageModel package = GetPackageByPackageName(purchase.PackageName);
            if (package == null || package.Amount == 0)
            {
                Logger.LogError($@"Uygulama içi satın alma hata!. Paket adından bakiye tipi alınırken null değer geldi. Acil kontrol ediniz. UserId: {UserId}
                                                                                                              PackageName: {purchase.PackageName}
                                                                                                              Platform: {purchase.Platform}
                                                                                                              ProductId: {purchase.ProductId}
                                                                                                              VerifyPurchase: {purchase.VerifyPurchase}
                                                                                                              Token: {purchase.Token}");
                return BadRequest(BalanceResource.PackagenameIsIncorrect);
            }

            #endregion Validations

            // TODO: google play istek atılıp token ve packet adı ile satın alım başarılı olmuşmu kontrol edilmeli

            bool isSuccess = await UpdateBalanceAsync(package.Amount, package.BalanceType);
            if (!isSuccess)
                return BadRequest(BalanceResource.ThePurchaseFailedPleaseEmailWithOurSupportTeam);

            #region Eğer özel ürün alıyorsa hem altın hem para ekleneceği için durum farklı

            if (package.IsSpecialOffer)
            {
                const decimal specialOfferGoldAmount = 10000.00m;

                isSuccess = await UpdateBalanceAsync(specialOfferGoldAmount, BalanceTypes.Gold);
                if (!isSuccess)
                {
                    Logger.LogError("{UserId} kullanıcı özel teklif ürünü aldı. Para yüklendi ancak altınlar yüklenemedi", UserId);
                }
            }

            #endregion Eğer özel ürün alıyorsa hem altın hem para ekleneceği için durum farklı

            AddPurchaseHistory(purchase, package);

            return Ok();
        }

        /// <summary>
        /// Bakiye güncelle
        /// </summary>
        /// <param name="amount">Eklenen bakiye miktarı</param>
        /// <param name="balanceType">Eklenecek bakiye tipi</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        private Task<bool> UpdateBalanceAsync(decimal amount, BalanceTypes balanceType)
        {
            return _balanceRepository.UpdateBalanceAsync(new ChangeBalanceModel
            {
                UserId = UserId,
                BalanceHistoryType = BalanceHistoryTypes.Buy,
                BalanceType = balanceType,
                Amount = amount
            });
        }

        /// <summary>
        /// Satın alma geçmişi ekler
        /// </summary>
        /// <param name="purchase">Satın alma bilgileri</param>
        /// <param name="amount">Satın alınan altın</param>
        /// <param name="balanceType">Satın alma itpi</param>
        private void AddPurchaseHistory(PurchaseModel purchase, PackageModel package)
        {
            _purchaseHistoryRepository.AddAsync(new PurchaseHistory
            {
                Amount = package.Amount,
                UserId = UserId,
                BalanceType = package.BalanceType,
                ProductId = purchase.ProductId,
                PackageName = purchase.PackageName,
                Token = purchase.Token,
                Platform = purchase.Platform,
                TransactionId = purchase.TransactionId,
                State = purchase.State,
                VerifyPurchase = purchase.VerifyPurchase,
            });
        }

        /// <summary>
        /// Paket isimlerine göre paket bilgilerini verir
        /// </summary>
        /// <param name="packageName">Google play/App store paket name</param>
        /// <returns>Bakiye tipi</returns>
        private PackageModel GetPackageByPackageName(string packageName)
        {
            if (Products.ContainsKey(packageName))
            {
                return Products[packageName];
            }

            return null;
        }

        #endregion Methods
    }
}
