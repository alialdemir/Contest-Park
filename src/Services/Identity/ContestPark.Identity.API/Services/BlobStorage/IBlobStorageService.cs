using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services.BlobStorage
{
    public interface IBlobStorageService
    {
        Task<bool> UploadFileToStorage(Stream fileStream, string fileName);
    }
}