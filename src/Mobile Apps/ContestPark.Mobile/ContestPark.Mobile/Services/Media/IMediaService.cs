using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Media
{
    public interface IMediaService
    {
        Task<Stream> ShowMediaActionSheet();
    }
}