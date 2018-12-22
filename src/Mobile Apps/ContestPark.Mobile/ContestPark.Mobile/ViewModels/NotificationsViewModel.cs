﻿using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Notification;
using ContestPark.Mobile.Services.Notification;
using ContestPark.Mobile.ViewModels.Base;
using System.Threading.Tasks;

namespace ContestPark.Mobile.ViewModels
{
    public class NotificationsViewModel : ViewModelBase<NotificationModel>
    {
        #region Private variable

        private readonly INotificationService _notificationService;

        #endregion Private variable

        #region Constructor

        public NotificationsViewModel(INotificationService notificationService)
        {
            Title = ContestParkResources.Notifications;

            _notificationService = notificationService;
        }

        #endregion Constructor

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = await _notificationService.NotificationListAsync(ServiceModel);

            await base.InitializeAsync();
            IsBusy = false;
        }

        #endregion Methods
    }
}