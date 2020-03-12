using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Notification;
using ContestPark.Mobile.Services.Notification;
using ContestPark.Mobile.ViewModels.Base;
using System.Threading.Tasks;

namespace ContestPark.Mobile.ViewModels
{
    public class NotificationViewModel : ViewModelBase<NotificationModel>
    {
        #region Private variables

        private readonly INotificationService _notificationService;

        #endregion Private variables

        #region Notifications

        public NotificationViewModel(INotificationService notificationService)
        {
            Title = ContestParkResources.Notifications;

            _notificationService = notificationService;
        }

        #endregion Notifications

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = await _notificationService.NotificationsAsync(ServiceModel);

            await base.InitializeAsync();

            IsBusy = true;
        }

        #endregion Methods
    }
}
