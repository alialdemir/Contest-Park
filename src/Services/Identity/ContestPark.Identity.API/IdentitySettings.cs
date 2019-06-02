namespace ContestPark.Identity.API
{
    public class IdentitySettings
    {
        public bool IsMigrateDatabase { get; set; }
        public bool UseCustomizationData { get; set; }

        public string AzureStoreAccountName { get; set; }
        public string AzureStoreAccountKey { get; set; }

        public string AzureStoreImageContainer { get; set; }
    }
}