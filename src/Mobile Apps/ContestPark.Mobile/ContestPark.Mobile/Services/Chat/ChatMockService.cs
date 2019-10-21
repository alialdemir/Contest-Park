using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Chat;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Chat
{
    public class ChatMockService : IChatService
    {
        public Task<ServiceModel<ChatDetailModel>> ChatDetailAsync(long conversationId, PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<ChatDetailModel>
            {
                Count = 3,
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize,
                Items = new List<ChatDetailModel>
                {
                       new ChatDetailModel
                        {
                           IsIncoming = true,
                            Date = DateTime.Now.AddDays(-30),
                            Message = "bu bir deneme mesajınız",
                            SenderId = "1111-1111-1111-1111",
                        },
                       new ChatDetailModel
                        {
                           IsIncoming = true,
                            Date = DateTime.Now.AddDays(-30),
                            Message = "Sellamm",
                            SenderId = "2222-2222-2222-2222",
                        },
                       new ChatDetailModel
                        {
                            Date = DateTime.Now.AddDays(-30),
                            Message = "123456789012345678901234567890123456789...",
                            SenderId = "3333-3333-3333-3333",
                        },
                       new ChatDetailModel
                        {
                           IsIncoming = true,
                            Date = DateTime.Now.AddDays(-30),
                            Message = "123456789012345678901234567890123456789...",
                            SenderId = "3333-3333-3333-3333",
                        },
                }
            });
        }

        public Task<bool> ChatSeenAsync()
        {
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(long conversationId)
        {
            return await Task.FromResult(true);
        }

        public Task<bool> SendMessage(MessageModel messageModel)
        {
            return Task.FromResult(true);
        }

        public async Task<ServiceModel<ChatModel>> UserChatList(PagingModel pagingModel)
        {
            await Task.Delay(2000);

            var chats = new ServiceModel<ChatModel>
            {
                Count = 3,
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize,
                Items = new List<ChatModel>
                {
                       new ChatModel
                        {
                            Date = DateTime.Now.AddDays(-30),
                            Message = "bu bir deneme mesajınız",
                            SenderUserId = "1111-1111-1111-1111",
                            UserFullName = "Ali Aldemir",
                            UserName = "witcherfearless",
                            UserProfilePicturePath = DefaultImages.DefaultProfilePicture,
                            VisibilityStatus = true
                        },
                       new ChatModel
                        {
                            Date = DateTime.Now.AddDays(-30),
                            Message = "Sellamm",
                            SenderUserId = "2222-2222-2222-2222",
                            UserFullName = "Ayşe Sönmez",
                            UserName = "deliayşe",
                            UserProfilePicturePath = DefaultImages.DefaultProfilePicture,
                            VisibilityStatus = false
                        },
                       new ChatModel
                        {
                            Date = DateTime.Now.AddDays(-30),
                            Message = "123456789012345678901234567890123456789...",
                            SenderUserId = "3333-3333-3333-3333",
                            UserFullName = "Meral belinç",
                            UserName = "belnc",
                            UserProfilePicturePath = DefaultImages.DefaultProfilePicture,
                            VisibilityStatus = true
                        }
                }
            };

            return chats;
        }

        public Task<int> UserChatVisibilityCountAsync()
        {
            return Task.FromResult(10);
        }
    }
}
