using FFmpeg.NET;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Services.Ffmpeg
{
    public class FfmpegService : IFfmpegService
    {
        #region Private Variables

        private readonly ILogger<FfmpegService> _logger;
        private readonly string _ffmpegPath;
        private readonly string _ffmpegTempPath = "";

        #endregion Private Variables

        #region Constructor

        public FfmpegService(IOptions<AdminSettings> options,
                             ILogger<FfmpegService> logger)
        {
            _logger = logger;

            _ffmpegPath = Path.Combine(options.Value.ClouldFrontUrl, "ffmpeg\\ffmpeg.exe");
            _ffmpegTempPath = $"{Path.GetTempPath()}ffmpeg.exe";
        }

        #endregion Constructor

        /// <summary>
        /// Dosyayı indirip stream olarak döndürür
        /// </summary>
        /// <param name="fileUrl">Dosya linki</param>
        /// <returns>Dosya stream</returns>
        private async Task DownloadFfmpegAsync()
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync(_ffmpegPath, _ffmpegTempPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ffmpeg dosyası indirme sırasında hata. Link: {_ffmpegPath}", ex.Message);
            }
        }

        /// <summary>
        /// Mp3 dosya linkini alıp ilk 10 saniyesini kesip döndüdürür
        /// </summary>
        /// <param name="file">Mp3 video link</param>
        /// <returns>MP3 file path</returns>
        public async Task<string> CutVideoAsync(string mp3Url)
        {
            if (string.IsNullOrEmpty(mp3Url))
                return string.Empty;

            if (!File.Exists(_ffmpegTempPath))
                await DownloadFfmpegAsync();

            string outputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".mp3");

            MediaFile inputFile = new MediaFile(mp3Url);
            MediaFile outputFile = new MediaFile(outputPath);

            Engine ffmpeg = new Engine(_ffmpegTempPath);
            ConversionOptions options = new ConversionOptions();

            options.CutMedia(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10));

            MediaFile mediaFile = await ffmpeg.ConvertAsync(inputFile, outputFile, options);

            if (mediaFile == null || mediaFile.FileInfo == null || string.IsNullOrEmpty(mediaFile.FileInfo.FullName))
            {
                _logger.LogWarning("Mp3 dosyası kesme işlemi başarısız.mp3 Url: {mp3Url}", mp3Url);
                return string.Empty;
            }

            return mediaFile.FileInfo.FullName;
        }
    }
}
