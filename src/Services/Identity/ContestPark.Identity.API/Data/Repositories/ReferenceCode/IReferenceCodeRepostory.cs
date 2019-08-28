namespace ContestPark.Identity.API.Data.Repositories.ReferenceCode
{
    public interface IReferenceCodeRepostory
    {
        void Insert(string code, string referenceUserId, string newUserId);
    }
}
