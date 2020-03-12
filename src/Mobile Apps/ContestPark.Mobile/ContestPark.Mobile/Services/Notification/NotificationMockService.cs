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
        public Task<ServiceModel<NotificationModel>> NotificationsAsync(PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<NotificationModel>()
            {
                Count = 3,
                HasNextPage = false,
                PageNumber = 1,
                PageSize = 10,
                Items = new List<NotificationModel>
                {
                    new NotificationModel
                    {
                      NotificationId= 14,
                      UserId= "96a89e5a-0ad1-43a6-9f5e-f53915ba6e20",
                      FullName= "Sinem Gürgen",
                      UserName= "sinem",
                      Description= "sinem postunu beğendi.",
                      ProfilePicturePath= "https://d2blqp3orvbj09.cloudfront.net/user.jpg",
                      IsNotificationSeen= false,
                      Date = DateTime.Now,
                      NotificationType= Enums.NotificationTypes.PostLike
                    },
                    new NotificationModel
                    {
                      NotificationId= 14,
                      UserId= "96a89e5a-0ad1-43a6-9f5e-f53915ba6e20",
                      FullName= "Sinem Gürgen",
                      UserName= "sinem",
                      Description= "sinem postunu beğendi.",
                      ProfilePicturePath= "https://d2blqp3orvbj09.cloudfront.net/user.jpg",
                      IsNotificationSeen= false,
                      Date = DateTime.Now,
                      NotificationType= Enums.NotificationTypes.PostLike
                    },
                    new NotificationModel
                    {
                      NotificationId= 14,
                      UserId= "96a89e5a-0ad1-43a6-9f5e-f53915ba6e20",
                      FullName= "Sinem Gürgen",
                      UserName= "sinem",
                      Description= "sinem postunu beğendi.",
                      ProfilePicturePath= "https://d2blqp3orvbj09.cloudfront.net/user.jpg",
                      IsNotificationSeen= false,
                      Date = DateTime.Now,
                      NotificationType= Enums.NotificationTypes.PostLike
                    },
                }
            });
        }
    }
}
