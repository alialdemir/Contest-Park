using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using ContestPark.Admin.API.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Services.Picture
{
    public class S3FileUploadService : IFileUploadService
    {
        #region Private Variables

        private readonly string bucketName = "contestpark";
        private readonly string clouldFrontUrl;

        private readonly IAmazonS3 _amazonS3;
        private readonly ILogger<S3FileUploadService> _logger;
        // private readonly IFfmpegService _ffmpegService;

        #endregion Private Variables

        #region Constructor

        public S3FileUploadService(ILogger<S3FileUploadService> logger,
                               IOptions<AdminSettings> options,
                               //    IFfmpegService ffmpegService,
                               IAmazonS3 amazonS3)
        {
            clouldFrontUrl = options.Value.ClouldFrontUrl;
            _logger = logger;
            // _ffmpegService = ffmpegService;
            _amazonS3 = amazonS3;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Verilen dosya yolundaki dosyayı stream olarak döndürür
        /// </summary>
        /// <param name="filePath">Dosya yolu</param>
        /// <returns>Stream file</returns>
        private Stream GetStream(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            byte[] fileByte = File.ReadAllBytes(filePath);

            if (File.Exists(filePath))
                File.Delete(filePath);

            if (fileByte == null || fileByte.Length == 0)
                return null;

            return new MemoryStream(fileByte);
        }

        /// <summary>
        /// Dosyayı indirip stream olarak döndürür
        /// </summary>
        /// <param name="fileUrl">Dosya linki</param>
        /// <returns>Dosya stream</returns>
        private async Task<string> DownloadFileAsync(Uri uri, string extension)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string filePath = $"{Path.GetTempPath()}contestparktempfile{extension}";

                    await webClient.DownloadFileTaskAsync(uri, filePath);

                    return filePath;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Dosya yükleme sırasında hata. Link: {uri.AbsoluteUri}", ex.Message);

                return null;
            }
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

        /// <summary>
        /// Soru resmi için S3 dosya yolunu verir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="extension">Dosya Uzantısı</param>
        /// <returns>Dosya yolu</returns>
        private string GetSubCategoryS3FilePath(short subCategoryId, string extension)
        {
            return $"questions/subcategoryId{subCategoryId}/{GetUniqFileName()}{extension}";
        }

        /// <summary>
        /// S3 üzerinden dosya siler
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFileToStorageAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            try
            {
                string key = path.Replace(clouldFrontUrl + "/", "");

                var response = await _amazonS3.DeleteObjectAsync(bucketName, key);

                return response.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Alt kategori resmi silme işlemi sırasında hata oluştu. file name: {path}", ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Resim yükleme
        /// </summary>
        /// <param name="fileStream">Resim stream</param>
        /// <param name="fileName">Dosyanın adı</param>
        /// <param name="userId">Hangi kullanıcının resmi</param>
        /// <returns>Resim url</returns>
        public async Task<string> UploadFileToStorageAsync(Stream fileStream, string fileName, string contentType, short subCategoryId)
        {
            if (CheckFileSize(fileStream.Length))
                return string.Empty;

            if (!CheckPictureExtension(contentType))
                return string.Empty;

            try
            {
                if (await CreateBucketIfNotExistsAsync() == false)
                    return string.Empty;

                TransferUtility transferUtility = new TransferUtility(_amazonS3);

                string extension = Path.GetExtension(fileName);
                string newFileName = $"categories/{GetUniqFileName()}{extension}";

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
                                                Key = "SubCategory Id",
                                                Value = subCategoryId.ToString()
                                            }
                                    }
                });

                return $"{clouldFrontUrl}/{newFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Alt kategori resmi yükleme işlemi sırasında hata oluştu. file name: {fileName}", ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Soru tipine göre dosya path verir
        /// </summary>
        /// <param name="fileUrl">Mp3 veya jpg dosya link</param>
        /// <param name="questionType">Soru tipi</param>
        /// <returns>İndirilen dosyanın path</returns>
        private async Task<string> GetFileStreamByQuestionType(string fileUrl, QuestionTypes questionType, string extension)
        {
            switch (questionType)
            {
                case QuestionTypes.Music:
                    bool isUrl = Uri.TryCreate(fileUrl, UriKind.Absolute, out Uri uriResult);
                    if (isUrl)
                    {
                        fileUrl = await DownloadFileAsync(uriResult, extension);
                    }

                    return fileUrl;
                //        return await _ffmpegService.CutVideoAsync(fileUrl);

                case QuestionTypes.Image:
                    return await DownloadFileAsync(new Uri(fileUrl), extension);
            }

            return null;
        }

        /// <summary>
        /// Resim yükleme
        /// </summary>
        /// <param name="fileStream">Resim stream</param>
        /// <param name="fileName">Dosyanın adı</param>
        /// <param name="userId">Hangi kullanıcının resmi</param>
        /// <returns>Resim url</returns>
        public async Task<string> UploadFileToStorageAsync(string fileUrl, short subCategoryId, QuestionTypes questionType)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return string.Empty;

            string extension = questionType == QuestionTypes.Music ? ".mp3" : ".png";

            string outputFilePath = await GetFileStreamByQuestionType(fileUrl, questionType, extension);
            if (string.IsNullOrEmpty(outputFilePath))
                return string.Empty;

            Stream fileStream = GetStream(outputFilePath);
            if (fileStream == null)
                return string.Empty;

            if (CheckFileSize(fileStream.Length))
                return string.Empty;

            try
            {
                if (await CreateBucketIfNotExistsAsync() == false)
                    return string.Empty;

                string newFileName = GetSubCategoryS3FilePath(subCategoryId, extension);

                TransferUtility transferUtility = new TransferUtility(_amazonS3);

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
            catch (Exception ex)
            {
                _logger.LogCritical($"Dosya yükleme sırasında hata oluştu. SubCategory Id: {subCategoryId} file name: {fileUrl}", ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Bucket daha önceden açılmamış ise açar
        /// </summary>
        /// <returns>Başarılı ise true değilse false</returns>
        private async Task<bool> CreateBucketIfNotExistsAsync()
        {
            try
            {
                bool isBucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
                if (isBucketExists)
                    return true;

                var response = await _amazonS3.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true,
                });

                return response.HttpStatusCode == HttpStatusCode.OK;
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

        #endregion Methods
    }
}
