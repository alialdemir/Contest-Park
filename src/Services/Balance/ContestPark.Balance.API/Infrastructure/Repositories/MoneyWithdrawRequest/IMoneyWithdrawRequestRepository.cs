namespace ContestPark.Balance.API.Infrastructure.Repositories.MoneyWithdrawRequest
{
    public interface IMoneyWithdrawRequestRepository
    {
        System.Threading.Tasks.Task<bool> Insert(Tables.MoneyWithdrawRequest money);
    }
}
