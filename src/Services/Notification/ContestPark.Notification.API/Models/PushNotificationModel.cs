namespace ContestPark.Notification.API.Models
{
    public class PushNotificationModel
    {
        /// <summary>
        /// Device id veya topics
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Bildirimde gönderilecek veri
        /// </summary>
        public PushNotificationMessageModel Notification { get; set; }
    }

    public class PushNotificationMessageModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
    }
}
