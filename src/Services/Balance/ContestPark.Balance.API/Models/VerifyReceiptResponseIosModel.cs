using Newtonsoft.Json;

namespace ContestPark.Balance.API.Models
{
    public class VerifyReceiptResponseIosModel
    {
        public ReceiptModel Receipt { get; set; }
        public int Status { get; set; }
    }

    public class ReceiptModel
    {
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("product_id")]
        public string ProductId { get; set; }
    }
}
