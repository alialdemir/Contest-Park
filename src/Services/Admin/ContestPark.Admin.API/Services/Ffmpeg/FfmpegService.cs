using FFmpeg.NET;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Services.Ffmpeg
{
    public class FfmpegService : IFfmpegService
    {
        #region Private Variables

        private readonly ILogger<FfmpegService> _logger;
        private readonly string _ffmpegTempPath;

        #endregion Private Variables

        #region Constructor

        public FfmpegService(IHostingEnvironment env,
                             ILogger<FfmpegService> logger)
        {
            _logger = logger;

            _ffmpegTempPath = Path.Combine(env.WebRootPath, "ffmpeg/ffmpeg.exe");
        }

        #endregion Constructor

        #region Methods

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
            {
                _logger.LogInformation("FFMPEG file bulunamadı.");

                return string.Empty;
            }

            if (!File.Exists(mp3Url))
            {
                _logger.LogInformation("mp3Url file bulunamadı.");
                return string.Empty;
            }

            string outputPath = $"{Path.GetTempPath()}{ Guid.NewGuid()}.mp3";
            try
            {
                MediaFile inputFile = new MediaFile(mp3Url);
                MediaFile outputFile = new MediaFile(outputPath);

                Engine ffmpeg = new Engine(_ffmpegTempPath);
                ConversionOptions options = new ConversionOptions();
                _logger.LogInformation("CutMedia öncesi");

                options.CutMedia(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10));

                _logger.LogInformation("ConvertAsync öncesi");

                MediaFile mediaFile = await ffmpeg.ConvertAsync(inputFile, outputFile, options);

                _logger.LogInformation("media file");

                if (mediaFile == null || mediaFile.FileInfo == null || string.IsNullOrEmpty(mediaFile.FileInfo.FullName))
                {
                    _logger.LogWarning("Mp3 dosyası kesme işlemi başarısız. mp3 Url: {mp3Url}", mp3Url);

                    return string.Empty;
                }
                _logger.LogInformation("FileInfo");
                _logger.LogInformation("mediaFile.FileInfo.FullName" + mediaFile.FileInfo.FullName);

                return mediaFile.FileInfo.FullName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ffmpeg error");
            }

            return string.Empty;
        }

        #endregion Methods
    }
}
