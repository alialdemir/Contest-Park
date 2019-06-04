using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using ContestPark.EventBus.Abstractions;
using ContestPark.Identity.API.IntegrationEvents.Events;

namespace ContestPark.Identity.API.Services.BlobStorage
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IdentitySettings _identitySettings;
        private readonly IEventBus _eventBus;

        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(IOptions<IdentitySettings> identitySettings,
                                  IEventBus eventBus,
                                  ILogger<BlobStorageService> logger)
        {
            _identitySettings = identitySettings.Value;
            _eventBus = eventBus;
            _logger = logger;
        }

        /// <summary>
        /// Resim yükleme
        /// </summary>
        /// <param name="fileStream">Resim stream</param>
        /// <param name="fileName">Dosyanın adı</param>
        /// <param name="userId">Hangi kullanıcının resmi</param>
        /// <returns></returns>
        public async Task<string> UploadFileToStorageAsync(Stream fileStream, string fileName, string userId)
        {
            if (CheckFileSize(fileStream.Length))
                return string.Empty;

            string extension = Path.GetExtension(fileName);
            if (!CheckPictureExtension(extension))
                return string.Empty;

            CloudBlockBlob blockBlob = null;
            try
            {
                string newFileName = GetUniqFileName() + extension;

                blockBlob = await GetCloudBlockBlobInstanceAsync(newFileName);
                if (await blockBlob.ExistsAsync())
                    return await UploadFileToStorageAsync(fileStream, fileName, userId);

                // Upload the file
                await blockBlob.UploadFromStreamAsync(fileStream);

                return blockBlob.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Dosya yüklerken hata oluştu user Id: {userId}", ex);

                if (blockBlob != null)// Dosya yükler ve hata oluşursa dosya boşa yer kaplamasın diye sildik
                    await blockBlob.DeleteIfExistsAsync();

                return string.Empty;
            }
        }

        private async Task<CloudBlockBlob> GetCloudBlockBlobInstanceAsync(string fileName, string containerName = "images")
        {
            // Create storagecredentials object by reading the values from the configuration (appsettings.json)
            StorageCredentials storageCredentials = new StorageCredentials(_identitySettings.AzureStoreAccountName, _identitySettings.AzureStoreAccountKey);
            // Create cloudstorage account by passing the storagecredentials
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            await container.CreateIfNotExistsAsync();

            // Get the reference to the block blob from the container
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            return blockBlob;
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

        /// <summary>
        /// Dosyayı blob storage üzerinden siler
        /// </summary>
        /// <param name="uri">Dosya uri</param>
        /// <returns></returns>
        public async Task<bool> DeleteFileAsync(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return false;

            try
            {
                string fileName = uri.Split("/images/").Last();
                if (string.IsNullOrEmpty(fileName))
                    return false;

                CloudBlockBlob blockBlob = await GetCloudBlockBlobInstanceAsync(fileName);
                if (blockBlob == null)
                    return false;

                bool isFileExists = await blockBlob.ExistsAsync();
                if (isFileExists)
                {
                    await blockBlob.DeleteAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Dosya silerken hata oluştu dosya uri: {uri}", ex);

                PublishDeleteFileEvent(uri);// TODO: burada bir queues sıkışması olabilir eğer sürekli hataya düşerse queues iş birikir

                return false;
            }
        }

        /// <summary>
        /// Eğer dosya silerken hata oluşursa tekrar dener
        /// </summary>
        /// <param name="uri"></param>
        private void PublishDeleteFileEvent(string uri)
        {
            var deleteFileIntegrationEvent = new DeleteFileIntegrationEvent(uri);
            _eventBus.Publish(deleteFileIntegrationEvent);
        }
    }
}