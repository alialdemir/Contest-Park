using ContestPark.Mobile.Models.Media;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Media
{
    public interface IMediaService
    {
        Task<MediaModel> GetPictureStream(string type);

        Task<MediaModel> ShowMediaActionSheet();
    }
}
