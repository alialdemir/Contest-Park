﻿using ContestPark.Core.Database.Models;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Block
{
    public interface IBlockRepository
    {
        Task<bool> BlockedAsync(string skirterUserId, string deterredUserId);

        bool BlockingStatus(string skirterUserId, string deterredUserId);

        Task<bool> UnBlockAsync(string skirterUserId, string deterredUserId);

        ServiceModel<Model.BlockModel> UserBlockedList(string SkirterUserId, PagingModel paging);
    }
}
