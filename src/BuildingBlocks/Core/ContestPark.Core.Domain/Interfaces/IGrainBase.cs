using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Core.Domain.Interfaces
{
    public interface IGrainBase : IGrainWithIntegerKey
    {
        Task OnDeactivateAsync();
    }
}