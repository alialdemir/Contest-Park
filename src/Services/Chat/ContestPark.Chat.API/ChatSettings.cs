namespace ContestPark.Chat.API
{
    public class ChatSettings
    {
        public string Redis { get; set; }
        public bool IsMigrateDatabase { get; set; }
        public string ConnectionString { get; set; }
    }
}
