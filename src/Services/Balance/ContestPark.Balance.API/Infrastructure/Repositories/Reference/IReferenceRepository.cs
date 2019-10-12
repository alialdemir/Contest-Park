using ContestPark.Balance.API.Models;

namespace ContestPark.Balance.API.Infrastructure.Repositories.Reference
{
    public interface IReferenceRepository
    {
        ReferenceModel IsCodeActive(string code, string userId);
    }
}
