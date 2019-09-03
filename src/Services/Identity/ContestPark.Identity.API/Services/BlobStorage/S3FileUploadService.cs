using Amazon.S3;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services.BlobStorage
{
    public class S3FileUploadService : IFileUploadService
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly ILogger<S3FileUploadService> _logger;

        public S3FileUploadService(IAmazonS3 amazonS3,
                                   ILogger<S3FileUploadService> logger)
        {
            _amazonS3 = amazonS3;
            _logger = logger;
        }

        public Task<bool> DeleteFileAsync(string uri)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadFileToStorageAsync(Stream fileStream, string fileName, string userId)
        {
            try
            {
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogCritical($"Dosya yükleme sırasında hata oluştu. User id: {userId} file name: {fileName}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Dosya yükleme sırasında hata oluştu. User id: {userId} file name: {fileName}", ex);
            }

            return Task.FromResult("");
        }

        /// <summary>
        /// Dosya boyutu yüklenebilecek boyuttamı
        /// </summary>
        /// <param name="size">Dosya boyutu</param>
        /// <returns>Yüklenemeyecekse true yüklenebilecekse false</returns>
        public bool CheckFileSize(long size)
        {
            decimal fileSizeMb = ((decimal)((size / 1024f) / 1024f));// convert byte to mb
            byte maximumFileSize = 4;// MB

            return fileSizeMb > maximumFileSize;// 4 mb'den büyük ise dosya boyutu  geçersizdir
        }

        /// <summary>
        /// Desteklenen resim formatı mı kontrol eder
        /// </summary>
        /// <param name="extension">Uzantı</param>
        /// <returns>Resim uzantısı destekleniyor ise true desteklenmiyorsafaslse</returns>
        public bool CheckPictureExtension(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".png":
                    return true;
            }

            return false;
        }
    }
}
