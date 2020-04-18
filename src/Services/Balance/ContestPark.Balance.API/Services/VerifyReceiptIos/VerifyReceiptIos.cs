using ContestPark.Balance.API.Models;
using ContestPark.Core.Services.RequestProvider;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Services.VerifyReceiptIos
{
    public class VerifyReceiptIos : IVerifyReceiptIos
    {
        #region Private variables

        private readonly IRequestProvider _requestProvider;
        private readonly BalanceSettings _balanceSettings;

        #endregion Private variables

        #region Constructor

        public VerifyReceiptIos(IRequestProvider requestProvider,
                                IOptions<BalanceSettings> options)
        {
            _requestProvider = requestProvider;
            _balanceSettings = options.Value;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Ios sandbox apisi üzerinden yapılan ödemenin doğru olup olmadığını kontrol eder
        /// </summary>
        /// <param name="token">Satın alma token bilgisi</param>
        /// <param name="receipt">Ürün bilgileri</param>
        /// <returns>Satın alma başarılı ise true değilse false döner</returns>
        public async Task<bool> Veriftasync(string token, ReceiptModel receipt)
        {
            if (string.IsNullOrEmpty(token) || receipt == null || string.IsNullOrEmpty(receipt.ProductId) || string.IsNullOrEmpty(receipt.TransactionId))
                return false;

            VerifyReceiptResponseIosModel response = await _requestProvider.PostAsync<VerifyReceiptResponseIosModel>("https://sandbox.itunes.apple.com/verifyReceipt", new VerifyReceiptIosModel
            {
                ReceiptData = token,
                Password = _balanceSettings.AppSpecificSharedSecret
            });

            return response != null
                && response.Status == 0// Sıfır başarılı demek
                && response.Receipt.ProductId == receipt.ProductId
                && response.Receipt.TransactionId == receipt.TransactionId;
        }

        #endregion Methods
    }
}
