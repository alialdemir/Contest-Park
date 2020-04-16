using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.LatestVersion
{
    public interface ILatestVersionService
    {
        Task IfNotUsingLatestVersionOpenInStore();
    }
}
