using ContestPark.Identity.API.Enums;
using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services.BlobStorage
{
    public interface IFileUploadService
    {
        bool CheckPictureExtension(string extension);

        bool CheckFileSize(long size);

        Task<string> UploadFileToStorageAsync(Stream fileStream, string fileName, string userId, string contentType, PictureTypes pictureType);

        Task DeleteFileAsync(string userId, string uri, PictureTypes pictureType);
    }
}
