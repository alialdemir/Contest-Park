using ContestPark.Core.Domain.Interfaces;
using System.Threading.Tasks;

namespace ContestPark.Domain.Category.Interfaces
{
    public interface ICategorySearchGrain : IGrainBase
    {
        Task Search();
    }
}