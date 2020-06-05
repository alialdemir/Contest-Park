using ContestPark.Admin.API.Model.Bet;
using ContestPark.Core.Database.Models;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.Bet
{
    public interface IBetRepository
    {
        Task<bool> AddAsync(Tables.Bet bet);

        ServiceModel<BetModel> GetBetList(PagingModel pagingModel);

        Task<bool> UpdateAsync(BetUpdateModel betUpdateModel);

        Task<bool> ClearAsync(byte betId);
    }
}
