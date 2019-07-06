using Newtonsoft.Json;

namespace ContestPark.Category.API
{
    public class CategorySettings
    {
        [JsonProperty("balanceUrl")]
        public string BalanceUrl { get; set; }

        public bool IsMigrateDatabase { get; set; }
        public string ConnectionString { get; set; }
    }
}
