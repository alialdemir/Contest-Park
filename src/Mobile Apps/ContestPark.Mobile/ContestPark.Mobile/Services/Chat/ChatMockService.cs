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
        public Task<bool> ChatSeenAsync()
        {
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(string receiverUserId)
        {
            return await Task.FromResult(true);
        }

        public async Task<ServiceModel<ChatModel>> UserChatList(PagingModel pagingModel)
        {
            await Task.Delay(2000);

            var categories = new ServiceModel<ChatModel>
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

            return categories;
        }

        public Task<int> UserChatVisibilityCountAsync()
        {
            return Task.FromResult(10);
        }
    }
}