using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.InAppBillingProduct;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.InAppBilling
{
    public class InAppBillingService : IInAppBillingService
    {
        #region Private variables

        private readonly IPageDialogService _pageDialogService;
        private readonly IInAppBilling billing;

        #endregion Private variables

        #region Constructor

        public InAppBillingService(IPageDialogService pageDialogService)
        {
            billing = CrossInAppBilling.Current;
            _pageDialogService = pageDialogService;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Google play de tanımlı ürün id'leri
        /// </summary>
        public string[] AndroidProductIds
        {
            get
            {
                return new string[7]
                                {
                                    "com.contestparkapp.app.250coins",
                                    "com.contestparkapp.app.1500coins",
                                    "com.contestparkapp.app.7000coins",
                                    "com.contestparkapp.app.20000coins",
                                    "com.contestparkapp.app.6",
                                    "com.contestparkapp.app.12",
                                    "com.contestparkapp.app.19"
                                };
            }
        }

        /// <summary>
        /// Ürün resimleri
        /// </summary>
        public Dictionary<string, string> ProductImages
        {
            get
            {
                var productImages = new Dictionary<string, string>();

                // TODO: ios eklendiği zaman burda if platform ios ise ios product ids dönünmeli

                for (int i = 0; i < AndroidProductIds.Length; i++)
                {
                    string productId = AndroidProductIds[i];

                    // eğer product id içinde dolar yazıyors varsa para paketleri yoksa altın paketleri gösterir
                    string svgName = productId.EndsWith("dolar") ? "contest_store_money_" : "contest_store_gold_";
                    productImages.Add(productId, $"assets/images/{svgName}{(i + 1).ToString()}.svg");
                }

                return productImages;
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
            try
            {
                if (!CrossInAppBilling.IsSupported)
                {
                    Debug.WriteLine("Telefonunuz uygulama içi satın almayı desteklemiyor.");

                    await ShowErrorDisplayAlertAsync(ContestParkResources.YourPhoneDoesNotSupportInAppPurchases);

                    return new List<InAppBillingProductModel>();
                }

                bool isConnected = await billing.ConnectAsync(ItemType.InAppPurchase);
                if (!isConnected)
                {
                    Debug.WriteLine("Uygulama için satın alma bağlantısı sağlanamadı.");

                    await ShowErrorDisplayAlertAsync(ContestParkResources.UnableToConnectToTheStore);

                    return new List<InAppBillingProductModel>();
                }

                IEnumerable<InAppBillingProduct> propductList = await billing.GetProductInfoAsync(ItemType.InAppPurchase, AndroidProductIds);
                if (propductList == null || propductList.Count() <= 0)
                {
                    Debug.WriteLine("Uygulama içi satın alınacak ürün listesi gelmedi.");

                    await ShowErrorDisplayAlertAsync(ContestParkResources.UnableToAccessTheProductList);

                    return new List<InAppBillingProductModel>();
                }

                return propductList.Select(product => new InAppBillingProductModel
                {
                    CurrencyCode = product.CurrencyCode,
                    Description = product.Description,
                    LocalizedPrice = product.LocalizedPrice,
                    ProductId = product.ProductId,
                    ProductName = product.Name.Replace(" (ContestPark)", ""),
                    Image = ProductImages.ContainsKey(product.ProductId) ? ProductImages[product.ProductId] : "",
                    BalanceTypes = product.Name.IndexOf("$") != -1 ? Enums.BalanceTypes.Money : Enums.BalanceTypes.Gold
                }).OrderBy(x => x.Image).ToList();
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
                await billing.DisconnectAsync();
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

                bool isExistsProductId = ProductImages.ContainsKey(productId);
                if (!isExistsProductId)
                {
                    await ShowErrorDisplayAlertAsync(ContestParkResources.InvalidProductId);

                    Debug.WriteLine($"Ürün satın alma sırasında geçersiz ürün id geldi. invalid product id: {productId}");

                    return null;
                }

                bool isConnected = await billing.ConnectAsync(ItemType.InAppPurchase);
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
                await billing.DisconnectAsync();
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
                    message = "GeneralError";
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
