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
            if (env == null)
                throw new Exception("env boşşşşşşşşşşşşşşşş");

            if (env.WebRootPath == null)
                throw new Exception("env.WebRootPath boşşşşşşşşşşşşşşşş");

            _ffmpegTempPath = Path.Combine(env.WebRootPath, "ffmpeg.exe");
            _logger = logger;
            if (_logger == null)
                throw new Exception("logger boşşşşşşşşşşşşşşşş");
            logger.LogWarning("ffpeg exists " + File.Exists(_ffmpegTempPath).ToString());
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

            if (File.Exists(_ffmpegTempPath))
                _logger.LogInformation("FFMOEG file bulunamadı.");

            if (File.Exists(mp3Url))
                _logger.LogInformation("mp3Url file bulunamadı.");

            string outputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".mp3");

            MediaFile inputFile = new MediaFile(mp3Url);
            MediaFile outputFile = new MediaFile(outputPath);

            FileInfo file = new FileInfo(_ffmpegTempPath);

            _logger.LogInformation("file info path " + file.FullName);

            Engine ffmpeg = new Engine(file.FullName);
            ConversionOptions options = new ConversionOptions();

            options.CutMedia(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10));

            MediaFile mediaFile = await ffmpeg.ConvertAsync(inputFile, outputFile, options);
            if (mediaFile == null || mediaFile.FileInfo == null || string.IsNullOrEmpty(mediaFile.FileInfo.FullName))
            {
                _logger.LogWarning("Mp3 dosyası kesme işlemi başarısız. mp3 Url: {mp3Url}", mp3Url);

                return string.Empty;
            }

            return mediaFile.FileInfo.FullName;
        }

        #endregion Methods
    }
}
