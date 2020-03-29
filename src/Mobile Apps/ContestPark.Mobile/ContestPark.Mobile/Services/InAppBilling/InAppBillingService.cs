using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.InAppBillingProduct;
using ContestPark.Mobile.Services.Cache;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.InAppBilling
{
    public class InAppBillingService : IInAppBillingService
    {
        #region Private variables

        private readonly IPageDialogService _pageDialogService;
        private readonly ICacheService _cacheService;
        private readonly IInAppBilling _billing;
        private const string _productCacheKey = "in-app-purche";

        #endregion Private variables

        #region Constructor

        public InAppBillingService(IPageDialogService pageDialogService,
                                   ICacheService cacheService)
        {
            _billing = CrossInAppBilling.Current;

            _pageDialogService = pageDialogService;
            _cacheService = cacheService;
        }

        #endregion Constructor

        #region Properties

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
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.6money",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney6,
                                            Description = ContestParkResources.ProductMoney6,
                                            Image =  "contest_store_money_5.svg"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.12money",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney12,
                                            Description = ContestParkResources.ProductMoney12,
                                            Image =  "contest_store_money_6.svg"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.19money",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney19,
                                            Description = ContestParkResources.ProductMoney19,
                                            Image =  "contest_store_money_7.svg"
                                        },
                                        // Gold
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.250Coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold250Name,
                                            Description = ContestParkResources.ProductGold250Description,
                                            Image =  "contest_store_gold_1.svg"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.1500Coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold1500Name,
                                            Description = ContestParkResources.ProductGold1500Description,
                                            Image =  "contest_store_gold_2.svg"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.7000Coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold7000Name,
                                            Description = ContestParkResources.ProductGold7000Description,
                                            Image =  "contest_store_gold_3.svg"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestpark.app.20000Coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold20000Name,
                                            Description = ContestParkResources.ProductGold20000Description,
                                            Image =  "contest_store_gold_4.svg"
                                        },
                                    };
                }

                return new List<InAppBillingProductModel>
                                    {
                                        // Money
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.6",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney6,
                                            Description = ContestParkResources.ProductMoney6,
                                            Image =  "contest_store_money_5.svg"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.12",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney12,
                                            Description = ContestParkResources.ProductMoney12,
                                            Image =  "contest_store_money_6.svg"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.19",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney19,
                                            Description = ContestParkResources.ProductMoney19,
                                            Image =  "contest_store_money_7.svg"
                                        },

                                        // Gold
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.250coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold250Name,
                                            Description = ContestParkResources.ProductGold250Description,
                                            Image =  "contest_store_gold_1.svg"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.1500coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold1500Name,
                                            Description = ContestParkResources.ProductGold1500Description,
                                            Image =  "contest_store_gold_2.svg"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.7000coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold7000Name,
                                            Description = ContestParkResources.ProductGold7000Description,
                                            Image =  "contest_store_gold_3.svg"
                                        },
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.20000coins",
                                            BalanceTypes=    BalanceTypes.Gold,
                                            ProductName = ContestParkResources.ProductGold20000Name,
                                            Description = ContestParkResources.ProductGold20000Description,
                                            Image =  "contest_store_gold_4.svg"
                                        },
                                    };
            }
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
                return await _cacheService.Get<List<InAppBillingProductModel>>(_productCacheKey);
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

                    await ShowErrorDisplayAlertAsync(ContestParkResources.UnableToConnectToTheStore);

                    return new List<InAppBillingProductModel>();
                }

                IEnumerable<InAppBillingProduct> propductList = await _billing.GetProductInfoAsync(ItemType.InAppPurchase, Products.Select(x => x.ProductId).ToArray());
                if (propductList == null || propductList.Count() <= 0)
                {
                    Debug.WriteLine("Uygulama içi satın alınacak ürün listesi gelmedi.");

                    await ShowErrorDisplayAlertAsync(ContestParkResources.UnableToAccessTheProductList);

                    return new List<InAppBillingProductModel>();
                }

                var products = propductList.Select(product => new InAppBillingProductModel
                {
                    CurrencyCode = product.CurrencyCode,
                    LocalizedPrice = product.LocalizedPrice,
                    Description = Products.FirstOrDefault(x => x.ProductId == product.ProductId).Description,
                    ProductId = product.ProductId,
                    ProductName = Products.FirstOrDefault(x => x.ProductId == product.ProductId).ProductName,
                    Image = Products.FirstOrDefault(x => x.ProductId == product.ProductId).Image,
                    BalanceTypes = Products.FirstOrDefault(x => x.ProductId == product.ProductId).BalanceTypes,
                }).OrderBy(x => x.Image).ToList();

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

                var purchase = await CrossInAppBilling.Current.PurchaseAsync(productId, ItemType.InAppPurchase, Guid.NewGuid().ToString());
                if (purchase == null)
                {
                    //Not purchased, alert the user
                    Debug.WriteLine("Satın alma işlemi denendi ama başarısız oldu.");
                }
                else
                {
                    //Purchased, save this information
                    var id = purchase.Id;
                    var token = purchase.PurchaseToken;
                    var state = purchase.State;

                    Debug.WriteLine($@"Satın alma işlemi gerçekleşti!\n
                                       Product Id: {purchase.ProductId}
                                       Id: {purchase.Id}
                                       Auto renewing: {purchase.AutoRenewing}
                                       Payload: {purchase.Payload}
                                       Purchase token: {purchase.PurchaseToken}
                                       Transaction date utc: {purchase.TransactionDateUtc}
                                       State: {purchase.State.ToString()}
                                       Consumption state: {purchase.ConsumptionState.ToString()} ");

                    return new InAppBillingPurchaseModel
                    {
                        AutoRenewing = purchase.AutoRenewing,
                        Id = purchase.Id,
                        Payload = purchase.Payload,
                        ProductId = purchase.ProductId,
                        PurchaseToken = purchase.PurchaseToken,
                        TransactionDateUtc = purchase.TransactionDateUtc,
                        State = purchase.State,
                        ConsumptionState = purchase.ConsumptionState
                    };
                }
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
                //      await billing.DisconnectAsync();
            }

            return null;
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
