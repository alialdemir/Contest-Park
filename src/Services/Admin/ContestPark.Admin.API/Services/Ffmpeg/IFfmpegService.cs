using System.Threading.Tasks;

namespace ContestPark.Admin.API.Services.Ffmpeg
{
    public interface IFfmpegService
    {
        Task<string> CutVideoAsync(string mp3Url);
    }
}
