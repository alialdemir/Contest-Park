﻿using FFmpeg.NET;
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
        private readonly string _ffmpegPath;

        #endregion Private Variables

        #region Constructor

        public FfmpegService(IHostingEnvironment hostingEnvironment,
                             ILogger<FfmpegService> logger)
        {
            _logger = logger;

            _ffmpegPath = Path.Combine(hostingEnvironment.WebRootPath, "ffmpeg\\ffmpeg.exe");
        }

        #endregion Constructor

        /// <summary>
        /// Mp3 dosya linkini alıp ilk 10 saniyesini kesip döndüdürür
        /// </summary>
        /// <param name="file">Mp3 video link</param>
        /// <returns>MP3 file path</returns>
        public async Task<string> CutVideoAsync(string mp3Url)
        {
            if (string.IsNullOrEmpty(mp3Url))
                return string.Empty;

            string outputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".mp3");

            MediaFile inputFile = new MediaFile(mp3Url);
            MediaFile outputFile = new MediaFile(outputPath);

            Engine ffmpeg = new Engine(_ffmpegPath);
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
