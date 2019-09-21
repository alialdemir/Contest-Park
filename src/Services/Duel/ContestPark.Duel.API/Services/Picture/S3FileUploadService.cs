using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Services.Picture
{
    public class S3FileUploadService : IFileUploadService
    {
        private readonly string bucketName = "contestpark";
        private readonly string clouldFrontUrl;

        private readonly IAmazonS3 _amazonS3;
        private readonly ILogger<S3FileUploadService> _logger;

        public S3FileUploadService(ILogger<S3FileUploadService> logger,
                                   IOptions<DuelSettings> identitySettings,
                                   IAmazonS3 amazonS3)
        {
            clouldFrontUrl = identitySettings.Value.ClouldFrontUrl;
            _logger = logger;
            _amazonS3 = amazonS3;
        }

        /// <summary>
        /// Dosyayı indirip stream olarak döndürür
        /// </summary>
        /// <param name="fileUrl">Dosya linki</param>
        /// <returns>Dosya stream</returns>
        private async Task<Stream> DownloadFileAsync(string fileUrl)
        {
            using (WebClient webClient = new WebClient())
            {
                string filePath = $"{Path.GetTempPath()}contestparkgeciciimage.png";// TODO: MÜZİK YÜKLERKEN BURASI PATLAR

                await webClient.DownloadFileTaskAsync(new Uri(fileUrl), filePath);

                byte[] fileByte = File.ReadAllBytes(filePath);

                if (File.Exists(filePath))
                    File.Delete(filePath);

                if (fileByte == null || fileByte.Length == 0)
                    return null;

                MemoryStream mem = new MemoryStream(fileByte);
                return mem;
            }
        }

        /// <summary>
        /// Resim yükleme
        /// </summary>
        /// <param name="fileStream">Resim stream</param>
        /// <param name="fileName">Dosyanın adı</param>
        /// <param name="userId">Hangi kullanıcının resmi</param>
        /// <returns>Resim url</returns>
        public async Task<string> UploadFileToStorageAsync(string fileUrl, short subCategoryId)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return string.Empty;

            Stream fileStream = await DownloadFileAsync(fileUrl);
            if (fileStream == null)
                return string.Empty;

            if (CheckFileSize(fileStream.Length))
                return string.Empty;

            try
            {
                if (await CreateBucketIfNotExistsAsync() == false)
                    return string.Empty;

                TransferUtility transferUtility = new TransferUtility(_amazonS3);

                string extension = ".png";// TODO: Burası müzik vs geldiği zaman patlar
                string newFileName = $"questions/{GetUniqFileName()}{extension}";

                await transferUtility.UploadAsync(new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    InputStream = fileStream,
                    Key = newFileName,
                    ContentType = "image/jpeg",// burası dinamik olmalı

                    StorageClass = S3StorageClass.Standard,
                    PartSize = fileStream.Length,
                    CannedACL = S3CannedACL.PublicRead,
                    TagSet = new List<Tag>
                                    {
                                        new Tag
                                            {
                                                Key = "SubCategory Id",
                                                Value = subCategoryId.ToString()
                                            }
                                    }
                });

                return $"{clouldFrontUrl}/{newFileName}";
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogCritical($"Dosya yükleme sırasında hata oluştu. SubCategory Id: {subCategoryId} file name: {fileUrl}", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Dosya yükleme sırasında hata oluştu. SubCategory Id: {subCategoryId} file name: {fileUrl}", ex.Message);
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
            catch (AmazonS3Exception ex)
            {
                _logger.LogCritical("Bucket oluşturma sırasında hata oluştu.", ex);
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

            string fileName = now.ToShortDateString().Replace(".", "") + now.ToLongTimeString().Replace(":", "") + Guid.NewGuid().ToString().Replace("-", "");

            return fileName;
        }

        /// <summary>
        /// Dosya boyutu yüklenebilecek boyuttamı
        /// </summary>
        /// <param name="size">Dosya boyutu</param>
        /// <returns>Yüklenemeyecekse true yüklenebilecekse false</returns>
        private bool CheckFileSize(long size)
        {
            decimal fileSizeMb = (decimal)(size / 1024f / 1024f);// convert byte to mb
            byte maximumFileSize = 4;// MB

            return fileSizeMb > maximumFileSize;// 4 mb'den büyük ise dosya boyutu  geçersizdir
        }
    }
}
