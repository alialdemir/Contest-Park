namespace ContestPark.Core.Services.RequestProvider
{
    public interface IRequestProvider
    {
        System.Threading.Tasks.Task<TResult> GetAsync<TResult>(string url);

        System.Threading.Tasks.Task<TResult> PostAsync<TResult>(string url, object data = null);
    }
}
