using ContestPark.Identity.API.Models;

namespace ContestPark.Identity.API.Data.Repositories.Reference
{
    public interface IReferenceRepository
    {
        ReferenceModel IsCodeActive(string code);
    }
}
