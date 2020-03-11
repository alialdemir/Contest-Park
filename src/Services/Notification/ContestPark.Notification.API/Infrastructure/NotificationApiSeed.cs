using ContestPark.Core.Database.Infrastructure;
using ContestPark.Notification.API.Enums;
using ContestPark.Notification.API.Infrastructure.Tables;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Infrastructure
{
    public class NotificationApiSeed : ContextSeedBase<NotificationApiSeed>
    {
        public async Task SeedAsync(IServiceProvider service, ILogger<NotificationApiSeed> logger)
        {
            var policy = CreatePolicy();

            Service = service;
            Logger = logger;

            await policy.ExecuteAsync(async () =>
            {
                await InsertDataAsync(new List<NotificationType>
                {
                     new NotificationType
                     {
                         IsActive = true,
                         NotificationTypeId = (byte)NotificationTypes.Follow,
                     },
                     new NotificationType
                     {
                         IsActive = true,
                         NotificationTypeId = (byte)NotificationTypes.PostLike,
                     },
                     new NotificationType
                     {
                         IsActive = true,
                         NotificationTypeId = (byte)NotificationTypes.PostComment,
                     },
                });

                await InsertDataAsync(new List<NotificationTypeLocalized>
                {
                    // Follow
                    new NotificationTypeLocalized
                    {
                        Description = "seni takip etmeye başladı.",
                        Language= Core.Enums.Languages.Turkish,
                        NotificationType = NotificationTypes.Follow
                    },
                    new NotificationTypeLocalized
                    {
                        Description = "started following you.",
                        Language= Core.Enums.Languages.English,
                        NotificationType = NotificationTypes.Follow
                    },

                    // Post like
                    new NotificationTypeLocalized
                    {
                        Description = "{0} postunu beğendi.",
                        Language= Core.Enums.Languages.Turkish,
                        NotificationType = NotificationTypes.PostLike
                    },
                    new NotificationTypeLocalized
                    {
                        Description = "{0} liked your post.",
                        Language= Core.Enums.Languages.English,
                        NotificationType = NotificationTypes.PostLike
                    },

                    // Post comment
                    new NotificationTypeLocalized
                    {
                        Description = "{0} yorum yaptı.",
                        Language= Core.Enums.Languages.Turkish,
                        NotificationType = NotificationTypes.PostComment
                    },
                    new NotificationTypeLocalized
                    {
                        Description = "{0} commented.",
                        Language= Core.Enums.Languages.English,
                        NotificationType = NotificationTypes.PostComment
                    },
                });

                await InsertDataAsync(new List<Tables.Notification>
                {
                    new Tables.Notification
                    {
                        IsNotificationSeen = false,
                        NotificationType = NotificationTypes.Follow,
                        WhoId = "2222-2222-2222-2222",
                        WhonId = "1111-1111-1111-1111",
                    },
                    new Tables.Notification
                    {
                        IsNotificationSeen = false,
                        NotificationType = NotificationTypes.PostComment,
                        WhoId = "2222-2222-2222-2222",
                        WhonId = "1111-1111-1111-1111",
                        PostId = 1,
                    },
                    new Tables.Notification
                    {
                        IsNotificationSeen = false,
                        NotificationType = NotificationTypes.PostComment,
                        WhoId = "2222-2222-2222-2222",
                        WhonId = "1111-1111-1111-1111",
                        PostId = 1,
                    },
                });
            });
        }
    }
}
