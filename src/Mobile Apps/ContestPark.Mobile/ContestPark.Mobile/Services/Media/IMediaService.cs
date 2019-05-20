using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Media
{
    public interface IMediaService
    {
        Task<Stream> GetPictureStream(string type);

        Task<Stream> ShowMediaActionSheet();
    }
}