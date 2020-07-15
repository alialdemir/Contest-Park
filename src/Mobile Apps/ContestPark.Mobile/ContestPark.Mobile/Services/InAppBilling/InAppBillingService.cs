using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Models.InAppBillingProduct;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.Cp;
using ImTools;
using Microsoft.AppCenter.Crashes;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Prism.Events;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.InAppBilling
{
    public class InAppBillingService : IInAppBillingService
    {
        #region Private variables

        private readonly IPageDialogService _pageDialogService;
        private readonly IBalanceService _balanceService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IAnalyticsService _analyticsService;
        private readonly ICacheService _cacheService;
        private readonly IInAppBilling _billing;
        private const string _productCacheKey = "in-app-purche";

        #endregion Private variables

        #region Constructor

        public InAppBillingService(IPageDialogService pageDialogService,
                                   IBalanceService balanceService,
                                   IEventAggregator eventAggregator,
                                   IAnalyticsService analyticsService,
                                   ICacheService cacheService)
        {
            _billing = CrossInAppBilling.Current;

            _pageDialogService = pageDialogService;
            _balanceService = balanceService;
            _eventAggregator = eventAggregator;
            _analyticsService = analyticsService;
            _cacheService = cacheService;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Özel ürün popupu için eklendi
        /// </summary>
        public string SpecialProductId
        {
            get
            {
                return Device.RuntimePlatform == Device.iOS
                    ? "com.contestpark.app.26money"
                    : "com.contestparkapp.app.26";
            }
        }

        /// <summary>
        /// Google play de tanımlı ürün id'leri
        /// </summary>
        private List<InAppBillingProductModel> Products
        {
            get
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    return new List<InAppBillingProductModel>
                                    {
                        // IOS
                                        // Money
                                        //new InAppBillingProductModel
                                        //{
                                        //    ProductId = "com.contestpark.app.6money",
                                        //    BalanceTypes=    BalanceTypes.Money,
                                        //    ProductName = ContestParkResources.ProductMoney6,
                                        //    Description = ContestParkResources.ProductMoney6,
                                        //    Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_money_5.svg?assembly=ContestPark.Mobile"
                                        //},
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.12money",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney12,
                                            Description = ContestParkResources.ProductMoney12,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_money_6.svg?assembly=ContestPark.Mobile"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.19money",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney19,
                                            Description = ContestParkResources.ProductMoney19,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_money_7.svg?assembly=ContestPark.Mobile"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.26money",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney26,
                                            Description = ContestParkResources.ProductMoney26,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.specialoffer.svg?assembly=ContestPark.Mobile",
                                            IsSpecialOffer = true,
                                            DiscountBalanceAmount  = 10000.00m
                                        },
                                        // Gold
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.250Coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold250Name,
                                            Description = ContestParkResources.ProductGold250Description,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_gold_1.svg?assembly=ContestPark.Mobile"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.1500Coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold1500Name,
                                            Description = ContestParkResources.ProductGold1500Description,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_gold_2.svg?assembly=ContestPark.Mobile"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.7000Coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold7000Name,
                                            Description = ContestParkResources.ProductGold7000Description,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_gold_3.svg?assembly=ContestPark.Mobile"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.20000Coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold20000Name,
                                            Description = ContestParkResources.ProductGold20000Description,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_gold_4.svg?assembly=ContestPark.Mobile"
                                        },
                                    };
                }

                // Android
                return new List<InAppBillingProductModel>
                                    {
                                        // Money
                                        //new InAppBillingProductModel
                                        //{
                                        //    ProductId = "com.contestparkapp.app.6",
                                        //    BalanceTypes=    BalanceTypes.Money,
                                        //    ProductName = ContestParkResources.ProductMoney6,
                                        //    Description = ContestParkResources.ProductMoney6,
                                        //    Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_money_5.svg?assembly=ContestPark.Mobile"
                                        //},
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.12",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney12,
                                            Description = ContestParkResources.ProductMoney12,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_money_6.svg?assembly=ContestPark.Mobile"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.19",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney19,
                                            Description = ContestParkResources.ProductMoney19,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_money_7.svg?assembly=ContestPark.Mobile"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.26",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney26,
                                            Description = ContestParkResources.ProductMoney26,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.specialoffer.svg?assembly=ContestPark.Mobile",
                                            IsSpecialOffer = true,
                                            DiscountBalanceAmount  =10.000m
                                        },

                                        // Gold
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.250coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold250Name,
                                            Description = ContestParkResources.ProductGold250Description,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_gold_1.svg?assembly=ContestPark.Mobile"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.1500coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold1500Name,
                                            Description = ContestParkResources.ProductGold1500Description,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_gold_2.svg?assembly=ContestPark.Mobile"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.7000coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold7000Name,
                                            Description = ContestParkResources.ProductGold7000Description,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_gold_3.svg?assembly=ContestPark.Mobile"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.20000coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold20000Name,
                                            Description = ContestParkResources.ProductGold20000Description,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_gold_4.svg?assembly=ContestPark.Mobile"
                                        },
                                    };
            }
        }

        public bool IsBusy { get; set; }

        private string PurchaseTokenFilePath
        {
            get { return Path.Combine(FileSystem.CacheDirectory, "cp.txt"); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// App stores ürünlerimizi döndürür
        /// </summary>
        /// <returns>Ürün listesi</returns>
        public async Task<List<InAppBillingProductModel>> GetProductInfoAsync()
        {
            if (!_cacheService.IsExpired(key: _productCacheKey))
            {
                return _cacheService.Get<List<InAppBillingProductModel>>(_productCacheKey);
            }

            try
            {
                if (!CrossInAppBilling.IsSupported)
                {
                    Debug.WriteLine("Telefonunuz uygulama içi satın almayı desteklemiyor.");

                    await ShowErrorDisplayAlertAsync(ContestParkResources.YourPhoneDoesNotSupportInAppPurchases);

                    return new List<InAppBillingProductModel>();
                }

                bool isConnected = await _billing.ConnectAsync(ItemType.InAppPurchase);
                if (!isConnected)
                {
                    Debug.WriteLine("Uygulama için satın alma bağlantısı sağlanamadı.");

                    ///////////  await ShowErrorDisplayAlertAsync(ContestParkResources.UnableToConnectToTheStore);

                    return new List<InAppBillingProductModel>();
                }

                IEnumerable<InAppBillingProduct> productList = await _billing.GetProductInfoAsync(ItemType.InAppPurchase, Products.Select(x => x.ProductId).ToArray());
                if (productList == null || productList.Count() <= 0)
                {
                    Debug.WriteLine("Uygulama içi satın alınacak ürün listesi gelmedi.");

                    await ShowErrorDisplayAlertAsync(ContestParkResources.UnableToAccessTheProductList);

                    return new List<InAppBillingProductModel>();
                }

                var products = (from product in productList
                                join p1 in Products on product.ProductId equals p1.ProductId
                                orderby p1.Image
                                let isDiscountPrice = p1.BalanceTypes == BalanceTypes.Money && !p1.IsSpecialOffer
                                select new InAppBillingProductModel
                                {
                                    CurrencyCode = product.CurrencyCode,
                                    LocalizedPrice = product.LocalizedPrice,
                                    IsSpecialOffer = p1.IsSpecialOffer,
                                    Description = p1.Description,
                                    ProductId = product.ProductId,
                                    ProductName = p1.ProductName,
                                    Image = p1.Image,
                                    BalanceTypes = p1.BalanceTypes,
                                    DiscountBalanceAmount = p1.DiscountBalanceAmount,
                                    RightText2TextDecorations = isDiscountPrice || p1.IsSpecialOffer ? TextDecorations.None : TextDecorations.Strikethrough,
                                    RightText2TextColor = isDiscountPrice ? Color.FromHex("#ff8800") : Color.Black,
                                    DiscountPrice = CalculatorDiscountPrice(product)
                                }).ToList();

                _cacheService.Add(_productCacheKey, products);

                return products;
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                await InAppBillingPurchaseExceptionHandler(purchaseEx.PurchaseError);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Uygulama içi satın alma hatası oluştu! Error Message: {ex.Message}");

                await ShowErrorDisplayAlertAsync(ContestParkResources.GlobalErrorMessage);
            }
            finally
            {
                //Disconnect, it is okay if we never connected
                await _billing.DisconnectAsync();
            }

            return new List<InAppBillingProductModel>();
        }

        /// <summary>
        /// Ürünlerin indirimli fiyatını hesaplar
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>İndirimli ürün fiyatı</returns>
        private string CalculatorDiscountPrice(InAppBillingProduct product)
        {
            var myProduct = Products.FirstOrDefault(x => x.ProductId == product.ProductId);

            if (product.ProductId == SpecialProductId)//Eğer bakiye tipi para ise 12.99 tl olan ürüne en çok satılan diye yazı ekler
                return ContestParkResources.BestSeller;

            if (string.IsNullOrEmpty(product.LocalizedPrice) || (myProduct.BalanceTypes == BalanceTypes.Money && !myProduct.IsSpecialOffer))
                return string.Empty;

            decimal price = Convert.ToDecimal(product.LocalizedPrice.Replace("₺", "").Replace("$", "").Replace("TRY", "").Trim());// Farklı para birimlerinde burası patlar

            price = ((price * 50 / 100) + price);

            return string.Format("{0:##.##}₺", price);// Fiyatın %20 fazlası
        }

        /// <summary>
        /// Ürün id'sine ait ürünü döndürür
        /// </summary>
        /// <param name="productId">Ürün id</param>
        /// <returns>Ürün bilgisi</returns>
        public async Task<InAppBillingProductModel> GetProductById(string productId)
        {
            return (await GetProductInfoAsync())
                                    .Where(product => product.ProductId == productId)
                                    .FirstOrDefault();
        }

        /// <summary>
        /// Uygulama içi ürün satın al
        /// </summary>
        /// <param name="productId">Ürün id</param>
        public async Task PurchaseProcessAsync(string productId)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            string productName = (await GetProductById(productId)).ProductName;

            _analyticsService.SendEvent("Enhanced ECommerce", "Impression", productName);

            InAppBillingPurchaseModel purchaseInfo = await PurchaseAsync(productId);
            if (purchaseInfo == null)
            {
                IsBusy = false;

                return;
            }

            #region Ios purchase token çok uzun olduğu için text dosyasına yazıp gönderiyoruz

            File.WriteAllText(PurchaseTokenFilePath, purchaseInfo.PurchaseToken);
            byte[] purchaseTokenByte = File.ReadAllBytes(PurchaseTokenFilePath);
            Stream purchaseTokenStream = new MemoryStream(purchaseTokenByte);

            #endregion Ios purchase token çok uzun olduğu için text dosyasına yazıp gönderiyoruz

            Crashes.TrackError(new Exception("Vertfy purchase"), new Dictionary<string, string>
            {
                {"VerifyPurchaselegth", purchaseInfo.VerifyPurchase.Length.ToString() },
                {"PayloadLength", purchaseInfo.Payload.Length.ToString() },
                {"IdLength", purchaseInfo.Id.Length.ToString() },
                {"VerifyPurchase", purchaseInfo.VerifyPurchase},
            });

            bool isSuccessGoldPurchase = await _balanceService.PurchaseAsync(new PurchaseModel
            {
                ProductId = purchaseInfo.ProductId,
                PackageName = purchaseInfo.ProductId,
                State = purchaseInfo.State,
                TransactionId = purchaseInfo.Id,
                Token = "none",
                Paylaod = purchaseInfo.Payload,
                VerifyPurchase = "test",//purchaseInfo.VerifyPurchase,
                Platform = GetCurrentPlatform(),
                File = purchaseTokenStream,
                FileName = "cp.txt"
            });
            if (isSuccessGoldPurchase)
            {
                PurchaseSuccess(purchaseInfo, productName);
            }
            else
            {
                await _pageDialogService.DisplayAlertAsync(ContestParkResources.Error,
                                                           ContestParkResources.PurchaseFail,
                                                           ContestParkResources.Okay);

                if (File.Exists(PurchaseTokenFilePath))
                    File.Delete(PurchaseTokenFilePath);

                SendProductEvent(purchaseInfo, "Remove From Cart", productName);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Satın alma işlemi başarılıysa yapılanlar
        /// </summary>
        /// <param name="purchaseInfo">Satılan paket bilgisi</param>
        /// <param name="productName">Paket adı</param>
        private void PurchaseSuccess(InAppBillingPurchaseModel purchaseInfo, string productName)
        {
            _pageDialogService.DisplayAlertAsync(ContestParkResources.Success,
                                                 ContestParkResources.ThePurchaseIsSuccessfulYourGoldBasBeenUploadedToYourAccount,
                                                 ContestParkResources.Okay);

            // Left menü'deki  altın miktarını güncelledik
            _eventAggregator
                 .GetEvent<GoldUpdatedEvent>()
                 .Publish();

            SendProductEvent(purchaseInfo, "Purchase", productName);

            if (File.Exists(PurchaseTokenFilePath))
                File.Delete(PurchaseTokenFilePath);
        }

        /// <summary>
        /// Product ga eventi gönderir
        /// </summary>
        /// <param name="purchaseInfo"></param>
        /// <param name="eventAction"></param>
        /// <param name="eventLabel"></param>
        private void SendProductEvent(InAppBillingPurchaseModel purchaseInfo, string eventAction, string eventLabel)
        {
            _analyticsService.SendEvent("Enhanced ECommerce", new Dictionary<string, string>
                {
                    { "ea", eventAction },
                    { "el", eventLabel },
                    { "ProductId", purchaseInfo.ProductId },
                    { "Id", purchaseInfo.Id },
                    { "AutoRenewing", purchaseInfo.AutoRenewing.ToString() },
                    { "Payload", purchaseInfo.Payload },
                    //{ "PurchaseToken", purchaseInfo.PurchaseToken },
                    { "TransactionDateUtc", purchaseInfo.TransactionDateUtc.ToLongDateString()},
                    { "State", purchaseInfo.State.ToString() },
                    { "ConsumptionState", purchaseInfo.ConsumptionState.ToString() },
                });
        }

        private Platforms GetCurrentPlatform()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android: return Platforms.Android;
                case Device.iOS: return Platforms.Ios;
            }

            return Platforms.Android;
        }

        /// <summary>
        /// Uygulama için ürün satın al
        /// </summary>
        /// <param name="productId">Ürün id</param>
        /// <returns>Başarılı ise ürün bilgileri değilse null ürün</returns>
        public async Task<InAppBillingPurchaseModel> PurchaseAsync(string productId)
        {
            try
            {
                if (!CrossInAppBilling.IsSupported)
                {
                    Debug.WriteLine("Telefonunuz uygulama içi satın almayı desteklemiyor.");

                    await ShowErrorDisplayAlertAsync(ContestParkResources.YourPhoneDoesNotSupportInAppPurchases);

                    return null;
                }

                bool isExistsProductId = Products.Any(product => product.ProductId == productId);
                if (!isExistsProductId)
                {
                    await ShowErrorDisplayAlertAsync(ContestParkResources.InvalidProductId);

                    Debug.WriteLine($"Ürün satın alma sırasında geçersiz ürün id geldi. invalid product id: {productId}");

                    return null;
                }

                bool isConnected = await _billing.ConnectAsync(ItemType.InAppPurchase);
                if (!isConnected)
                {
                    Debug.WriteLine("Uygulama için satın alma bağlantısı sağlanamadı.");

                    await ShowErrorDisplayAlertAsync(ContestParkResources.UnableToConnectToTheStore);

                    return null;
                }

                await RestorePurchase(productId);// Eğer ürün daha önceden alınmısa onu tüketildi olarak bildiriyoruz ki ikinci defa satın alınabilsin

                string verifyPurchase = Guid.NewGuid().ToString();

                var purchase = await _billing.PurchaseAsync(productId, ItemType.InAppPurchase, verifyPurchase);
                if (purchase == null)
                {
                    //Not purchased, alert the user
                    Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Satın alma işlemi denendi ama başarısız oldu.");
                }
                else
                {
                    //Purchased, save this information

                    Microsoft.AppCenter.Analytics.Analytics.TrackEvent($@"Satın alma işlemi gerçekleşti!", new Dictionary<string, string>
                {
                    { "ProductId", purchase.ProductId },
                    { "Id", purchase.Id },
                    { "AutoRenewing", purchase.AutoRenewing.ToString() },
                    { "Payload", purchase.Payload },
                    //{ "PurchaseToken", purchaseInfo.PurchaseToken },
                    { "TransactionDateUtc", purchase.TransactionDateUtc.ToLongDateString()},
                    { "State", purchase.State.ToString() },
                    { "ConsumptionState", purchase.ConsumptionState.ToString() },
                });

                    return new InAppBillingPurchaseModel
                    {
                        AutoRenewing = purchase.AutoRenewing,
                        Id = purchase.Id,
                        Payload = purchase.Payload,
                        ProductId = purchase.ProductId,
                        PurchaseToken = purchase.PurchaseToken,
                        TransactionDateUtc = purchase.TransactionDateUtc,
                        State = purchase.State,
                        ConsumptionState = purchase.ConsumptionState,
                        VerifyPurchase = verifyPurchase
                    };
                }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                await InAppBillingPurchaseExceptionHandler(purchaseEx.PurchaseError);
            }
            catch (Exception ex)
            {
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent($"Uygulama içi satın alma hatası oluştu! Error Message: {ex.Message}");

                await ShowErrorDisplayAlertAsync(ContestParkResources.GlobalErrorMessage);
            }
            finally
            {
                //Disconnect, it is okay if we never connected
                await _billing.DisconnectAsync();
            }

            return null;
        }

        /// <summary>
        /// Parametreden gelen ürün id daha önce satın alınmışsa onu tüketildi yapar
        /// </summary>
        /// <param name="productId">Ürün id</param>
        private async Task RestorePurchase(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                return;

            IEnumerable<InAppBillingPurchase> appBillingPurchases = await _billing.GetPurchasesAsync(ItemType.InAppPurchase);
            if (appBillingPurchases == null || !appBillingPurchases.Any(x => x.ProductId == productId))
                return;

            InAppBillingPurchase inAppBillingPurchase = appBillingPurchases.FirstOrDefault(x => x.ProductId == productId);
            if (inAppBillingPurchase != null)
            {
                await _billing.ConsumePurchaseAsync(inAppBillingPurchase.ProductId, inAppBillingPurchase.PurchaseToken);
            }
        }

        /// <summary>
        /// Satın almada oluşan hataları yakalar
        /// </summary>
        /// <param name="purchaseError">Error tipi</param>
        /// <returns></returns>
        private async Task InAppBillingPurchaseExceptionHandler(PurchaseError purchaseError)
        {
            var message = string.Empty;
            switch (purchaseError)
            {
                case PurchaseError.BillingUnavailable:
                    message = "BillingUnavailable";
                    break;

                case PurchaseError.DeveloperError:
                    message = "DeveloperError";
                    break;

                case PurchaseError.ItemUnavailable:
                    message = "ItemUnavailable";
                    break;

                case PurchaseError.GeneralError:
                    //      message = "GeneralError";
                    break;

                case PurchaseError.UserCancelled:
                    //message = "UserCancelled.";
                    break;

                case PurchaseError.AppStoreUnavailable:
                    message = "AppStoreUnavailable.";
                    break;

                case PurchaseError.PaymentNotAllowed:
                    message = "PaymentNotAllowed.";
                    break;

                case PurchaseError.PaymentInvalid:
                    message = "PaymentInvalid";
                    break;

                case PurchaseError.InvalidProduct:
                    message = "InvalidProduct";
                    break;

                case PurchaseError.ProductRequestFailed:
                    message = "ProductRequestFailed";
                    break;

                case PurchaseError.RestoreFailed:
                    message = "RestoreFailed";
                    break;

                case PurchaseError.ServiceUnavailable:
                    message = "ServiceUnavailable";
                    break;
            }

            Debug.WriteLine($"Satıl alma hatası Error Code: {(byte)purchaseError} PurchaseError: {purchaseError.ToString()}");

            await ShowErrorDisplayAlertAsync(message);
        }

        private async Task ShowErrorDisplayAlertAsync(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            await _pageDialogService?.DisplayAlertAsync(
                ContestParkResources.Error,
                message,
                ContestParkResources.Okay);
        }

        #endregion Methods
    }
}
