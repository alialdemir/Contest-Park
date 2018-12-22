using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Notification;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Notification
{
    public class NotificationMockService : INotificationService
    {
        public Task<ServiceModel<NotificationModel>> NotificationListAsync(PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<NotificationModel>
            {
                Count = 1,
                Items = new List<NotificationModel>
                 {
                    GetNotificationModel(),
                    GetNotificationModel(),
                    GetNotificationModel(),
                    GetNotificationModel(),
                    GetNotificationModel(),
                 }
            });
        }

        private NotificationModel GetNotificationModel()
        {
            return new NotificationModel
            {
                NotificationId = 1,
                Date = DateTime.Now.AddDays(-30),
                NotificationType = Enums.NotificationTypes.Contest,
                PicturePath = DefaultImages.DefaultProfilePicture,
                Text = "Seninle {yarisma} yarışmasında düello yaptı. Ona karşı koy!",
                WhoFullName = "Elif Öz",
                WhoUserName = "elifoz"
            };
        }
    }
}