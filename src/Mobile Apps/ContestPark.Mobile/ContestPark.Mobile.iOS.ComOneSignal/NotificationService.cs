using Com.OneSignal;
using Foundation;
using System;
using UserNotifications;

namespace ContestPark.Mobile.iOS.ComOneSignal
{
    [Register("NotificationService")]
    public class NotificationService : UNNotificationServiceExtension
    {
        private Action<UNNotificationContent> ContentHandler { get; set; }
        private UNMutableNotificationContent BestAttemptContent { get; set; }
        private UNNotificationRequest ReceivedRequest { get; set; }

        protected NotificationService(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
        {
            ReceivedRequest = request;
            ContentHandler = contentHandler;
            BestAttemptContent = (UNMutableNotificationContent)request.Content.MutableCopy();

            (OneSignal.Current as OneSignalImplementation).DidReceiveNotificationExtensionRequest(request, BestAttemptContent);

            ContentHandler(BestAttemptContent);
        }

        public override void TimeWillExpire()
        {
            // Called just before the extension will be terminated by the system.
            // Use this as an opportunity to deliver your "best attempt" at modified content, otherwise the original push payload will be used.

            (OneSignal.Current as OneSignalImplementation).ServiceExtensionTimeWillExpireRequest(ReceivedRequest, BestAttemptContent);

            ContentHandler(BestAttemptContent);
        }
    }
}
