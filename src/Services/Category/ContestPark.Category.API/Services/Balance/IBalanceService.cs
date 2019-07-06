namespace ContestPark.Category.API.Services.Balance
{
    public interface IBalanceService
    {
        System.Threading.Tasks.Task<Model.BalanceModel> GetBalance(string userId, IntegrationEvents.Events.BalanceTypes balanceType);
    }
}