using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services.BlobStorage
{
    public interface IBlobStorageService
    {
        bool CheckPictureExtension(string extension);

        bool CheckFileSize(long size);

        Task<string> UploadFileToStorageAsync(Stream fileStream, string fileName, string userId);

        Task<bool> DeleteFileAsync(string uri);
    }
}