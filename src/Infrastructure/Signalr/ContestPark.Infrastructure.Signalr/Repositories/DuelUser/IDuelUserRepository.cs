using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Signalr.Repositories.DuelUser
{
    public interface IDuelUserRepository
    {
        Task<bool> InsertAsync(Entities.DuelUser duelUser);

        Task<List<Entities.DuelUser>> GetAllAsync();

        Task<Entities.DuelUser> GetDuelUserAsync(string userId, Int16 subCategoryId, int bet);

        Task<bool> DeleteAsync(Entities.DuelUser duelUser);

        void ClearExpiredDuelUserList();
    }
}