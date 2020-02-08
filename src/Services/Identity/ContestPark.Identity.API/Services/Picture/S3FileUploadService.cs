using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using ContestPark.EventBus.Abstractions;
using ContestPark.Identity.API.Data.Repositories.Picture;
using ContestPark.Identity.API.Enums;
using ContestPark.Identity.API.IntegrationEvents.Events;
using ContestPark.Identity.API.Services.BlobStorage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services.Picture
{
    public class S3FileUploadService : IFileUploadService
    {
        private readonly string bucketName = "contestpark";
        private readonly string clouldFrontUrl;

        private readonly IEventBus _eventBus;
        private readonly IAmazonS3 _amazonS3;
        private readonly ILogger<S3FileUploadService> _logger;
        private readonly IPictureRepository _pictureRepository;

        public S3FileUploadService(ILogger<S3FileUploadService> logger,
                                   IPictureRepository pictureRepository,
                                   IEventBus eventBus,
                                   IOptions<IdentitySettings> identitySettings,
                                   IAmazonS3 amazonS3)
        {
            _eventBus = eventBus;
            clouldFrontUrl = identitySettings.Value.ClouldFrontUrl;
            _logger = logger;
            _pictureRepository = pictureRepository;
            _amazonS3 = amazonS3;
        }

        /// <summary>
        /// Resim url'sini alıp user id ile birlikte pictures tablosuna ekler
        /// Başka(post) yerlerde bu resim kullanılıyorsa oralarda link kırık gözükmesin diye resmi direk silmiyoruz
        /// </summary>
        /// <param name="userId">Resmin sahibinin user id</param>
        /// <param name="uri">Resim url</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public void DeleteFile(string userId, string uri, PictureTypes pictureType)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(uri))
                return;

            try
            {
                bool isSuccess = _pictureRepository.Insert(userId, uri, pictureType);
                if (!isSuccess)
                {
                    PublishDeleteFileEvent(userId, uri, pictureType);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Dosya silerken hata oluştu user Id: {userId}", ex.Message);

                PublishDeleteFileEvent(userId, uri, pictureType);
            }
        }

        /// <summary>
        /// Eğer dosya silerken hata oluşursa tekrar dener
        /// </summary>
        /// <param name="uri"></param>
        private void PublishDeleteFileEvent(string userId, string uri, PictureTypes pictureType)
        {
            var deleteFileIntegrationEvent = new DeleteFileIntegrationEvent(userId, uri, pictureType);
            _eventBus.Publish(deleteFileIntegrationEvent);
        }

        /// <summary>
        /// Resim yükleme
        /// </summary>
        /// <param name="fileStream">Resim stream</param>
        /// <param name="fileName">Dosyanın adı</param>
        /// <param name="userId">Hangi kullanıcının resmi</param>
        /// <returns>Resim url</returns>
        public async Task<string> UploadFileToStorageAsync(Stream fileStream, string fileName, string userId, string contentType, PictureTypes pictureType)
        {
            //if (CheckFileSize(fileStream.Length))
            //    return string.Empty;

            if (!CheckPictureExtension(contentType))
                return string.Empty;

            try
            {
                if (await CreateBucketIfNotExistsAsync() == false)
                    return string.Empty;

                TransferUtility transferUtility = new TransferUtility(_amazonS3);

                string extension = Path.GetExtension(fileName);
                string newFileName = $"{pictureType.ToString().ToLower()}/{GetUniqFileName()}{extension}";

                await transferUtility.UploadAsync(new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    InputStream = fileStream,
                    Key = newFileName,
                    ContentType = contentType,

                    StorageClass = S3StorageClass.Standard,
                    PartSize = fileStream.Length,
                    CannedACL = S3CannedACL.PublicRead,
                    TagSet = new List<Tag>
                                    {
                                        new Tag
                                            {
                                                Key = "User id",
                                                Value = userId
                                            }
                                    }
                });

                return $"{clouldFrontUrl}/{newFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Dosya yükleme sırasında hata oluştu. User id: {userId} file name: {fileName}", ex.Message);
            }

            return string.Empty;
        }

        private async Task<bool> CreateBucketIfNotExistsAsync()
        {
            try
            {
                bool isBucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
                if (isBucketExists)
                    return true;

                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true,
                };

                var response = await _amazonS3.PutBucketAsync(putBucketRequest);

                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Bucket oluşturma sırasında hata oluştu.", ex);
            }

            return false;
        }

        /// <summary>
        /// Uniq dosya adı oluşturur
        /// format: gün + ay + yıl + saat + dakika + saniye + guid key
        /// </summary>
        /// <returns></returns>
        private string GetUniqFileName()
        {
            DateTime now = DateTime.Now;

            string fileName = now.ToShortDateString().Replace(".", "") + now.ToLongTimeString().Replace(":", "") + Guid.NewGuid().ToString().Replace("-", "").Replace("AM", "");

            return fileName.Replace(" ", "");
        }

        /// <summary>
        /// Dosya boyutu yüklenebilecek boyuttamı
        /// </summary>
        /// <param name="size">Dosya boyutu</param>
        /// <returns>Yüklenemeyecekse true yüklenebilecekse false</returns>
        public bool CheckFileSize(long size)
        {
            decimal fileSizeMb = (decimal)(size / 1024f / 1024f);// convert byte to mb
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
                case "image/jpeg":
                case "image/png":
                    return true;
            }

            return false;
        }
    }
}
