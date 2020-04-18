using Newtonsoft.Json;

namespace ContestPark.Balance.API.Models
{
    public class VerifyReceiptIosModel
    {
        [JsonProperty("receipt-data")]
        public string ReceiptData { get; set; }

        public string Password { get; set; }
    }
}
