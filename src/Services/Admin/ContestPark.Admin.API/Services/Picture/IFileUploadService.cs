using System.Threading.Tasks;

namespace ContestPark.Admin.API.Services.Picture
{
    public interface IFileUploadService
    {
        Task<bool> DeleteFileToStorageAsync(string path);
        Task<string> UploadFileToStorageAsync(string fileUrl, short subCategoryId);

        Task<string> UploadFileToStorageAsync(System.IO.Stream fileStream, string fileName, string contentType, short subCategoryId);
    }
}
