namespace ContestPark.Notification.API
{
    public class NotificationSettings
    {
        public bool IsMigrateDatabase { get; set; }
        public string ConnectionString { get; set; }
        public string Redis { get; set; }

        public string AwsSmsKeyId { get; set; }
        public string AwsSmsSecret { get; set; }
    }
}
