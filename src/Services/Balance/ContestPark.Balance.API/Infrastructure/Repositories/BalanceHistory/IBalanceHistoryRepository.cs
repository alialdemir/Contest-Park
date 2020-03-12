namespace ContestPark.Balance.API.Infrastructure.Repositories.BalanceHistory
{
    public interface IBalanceHistoryRepository
    {
        bool IsReward(string userId);
    }
}
