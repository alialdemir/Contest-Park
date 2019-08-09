namespace ContestPark.Duel.API
{
    public class DuelSettings
    {
        public bool IsMigrateDatabase { get; set; }
        public string ConnectionString { get; set; }
        public string Redis { get; set; }
        public string FollowUrl { get; set; }
        public string SubCategoryUrl { get; set; }
        public byte BetCommission { get; set; } = 10;
    }
}
