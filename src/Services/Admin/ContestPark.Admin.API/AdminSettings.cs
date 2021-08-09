namespace ContestPark.Admin.API
{
    public class AdminSettings
    {
        public string ClouldFrontUrl { get; set; }

        public string SpotifyClientId { get; set; }
        public string SpotifySecretId { get; set; }
        public bool IsMigrateDatabase { get; set; }
        public string ConnectionString { get; set; }
    }
}
