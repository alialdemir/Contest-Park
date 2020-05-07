using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.InAppBillingProduct;
using ContestPark.Mobile.Services.Cache;
using ImTools;
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
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_money_5.svg?assembly=ContestPark.Mobile"
                                        },
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
                                            DiscountBalanceAmount  = 10.000m
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

                return new List<InAppBillingProductModel>
                                    {
                                        // Money
                                        new InAppBillingProductModel
                                        {
                                            ProductId = "com.contestparkapp.app.6",
                                            BalanceTypes=    BalanceTypes.Money,
                                            ProductName = ContestParkResources.ProductMoney6,
                                            Description = ContestParkResources.ProductMoney6,
                                            Image =  "resource://ContestPark.Mobile.Common.Images.contest_store_money_5.svg?assembly=ContestPark.Mobile"
                                        },
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
                                    RightText2TextDecorations = isDiscountPrice ? TextDecorations.None : TextDecorations.Strikethrough,
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
        /// <returns></returns>
        private string CalculatorDiscountPrice(InAppBillingProduct product)
        {
            var myProduct = Products.FirstOrDefault(x => x.ProductId == product.ProductId);

            if (product.ProductId == "com.contestpark.app.12money")//Eğer bakiye tipi para ise 12.99 tl olan ürüne en çok satılan diye yazı ekler
                return ContestParkResources.BestSeller;

            if (string.IsNullOrEmpty(product.LocalizedPrice) || (myProduct.BalanceTypes == BalanceTypes.Money && !myProduct.IsSpecialOffer))
                return string.Empty;

            decimal price = Convert.ToDecimal(product.LocalizedPrice.Replace("₺", "").Replace("$", "").Replace("TRY", "").Trim());// Farklı para birimlerinde burası patlar

            price = ((price * 50 / 100) + price);

            return string.Format("{0:##.##}₺", price);// Fiyatın %20 fazlası
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
                string verifyPurchase = Guid.NewGuid().ToString();

                var purchase = await _billing.PurchaseAsync(productId, ItemType.InAppPurchase, verifyPurchase);
                if (purchase == null)
                {
                    //Not purchased, alert the user
                    Debug.WriteLine("Satın alma işlemi denendi ama başarısız oldu.");
                }
                else
                {
                    //Purchased, save this information

                    Debug.WriteLine($@"Satın alma işlemi gerçekleşti!\n
                                       Product Id: {purchase.ProductId}
                                       Id: {purchase.Id}
                                       Auto renewing: {purchase.AutoRenewing}
                                       Payload: {purchase.Payload}
                                       Purchase token: {purchase.PurchaseToken}
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
                Debug.WriteLine($"Uygulama içi satın alma hatası oluştu! Error Message: {ex.Message}");

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
        /// Android tarafında satın alınan öğeyi tüketildi yapar
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="purchaseToken"></param>
        /// <returns></returns>
        public Task<InAppBillingPurchase> ConsumePurchaseAsync(string productId, string purchaseToken)
        {
            if (Device.RuntimePlatform == Device.Android
                && !string.IsNullOrEmpty(productId)
                && !string.IsNullOrEmpty(purchaseToken))
            {
                return _billing.ConsumePurchaseAsync(productId, purchaseToken);
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
