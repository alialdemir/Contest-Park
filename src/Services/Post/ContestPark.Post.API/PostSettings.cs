namespace ContestPark.Post.API
{
    public class PostSettings
    {
        public string Audience { get; set; }
        public string identityUrl { get; set; }
        public string Redis { get; set; }
        public bool IsMigrateDatabase { get; set; }
        public string ConnectionString { get; set; }
    }
}
