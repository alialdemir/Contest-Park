using System.Threading.Tasks;

namespace ContestPark.Duel.API.Services.Picture
{
    public interface IFileUploadService
    {
        Task<string> UploadFileToStorageAsync(string fileUrl, short subCategoryId);
    }
}
