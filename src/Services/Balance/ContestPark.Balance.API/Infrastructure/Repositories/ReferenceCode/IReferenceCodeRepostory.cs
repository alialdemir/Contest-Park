using System.Threading.Tasks;

namespace ContestPark.Balance.API.Infrastructure.Repositories.ReferenceCode
{
    public interface IReferenceCodeRepostory
    {
        Task<bool> Insert(string code, string referenceUserId, string newUserId);
    }
}
