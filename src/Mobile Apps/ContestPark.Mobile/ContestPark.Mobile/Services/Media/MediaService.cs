using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Media;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Media
{
    /// <summary>
    /// Defines the <see cref="MediaService"/>
    /// </summary>
    public class MediaService : IMediaService
    {
        #region Fields

        /// <summary>
        /// Defines the _pageDialogService
        /// </summary>
        private readonly IPageDialogService _pageDialogService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaService"/> class.
        /// </summary>
        /// <param name="pageDialogService">The pageDialogService <see cref="IPageDialogService"/></param>
        public MediaService(IPageDialogService pageDialogService)
        {
            _pageDialogService = pageDialogService;
        }

        #endregion Constructors

        #region Methods

        public async Task<MediaModel> GetPictureStream(string type)
        {
            if (string.Equals(type, ContestParkResources.ChooseFromLibrary))
            {
                bool permissionStatus = await CheckPermissionStatusAsync(Permission.Storage);
                if (!permissionStatus)
                {
                    await ShowErrorMessage();
                    return null;
                }

                return await ChooseFromLibraryAsync();
            }
            else if (string.Equals(type, ContestParkResources.TakeAPhoto))
            {
                bool permissionStatus = await CheckPermissionStatusAsync(Permission.Camera);
                if (!permissionStatus)
                {
                    await ShowErrorMessage();
                    return null;
                }

                return await TakeAPhotoAsync();
            }

            return null;
        }

        /// <summary>
        /// The ShowMediaActionSheet
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task<MediaModel> ShowMediaActionSheet()
        {
            string selected = await _pageDialogService?.DisplayActionSheetAsync(ContestParkResources.ChooseAnAction,
                                                                                    ContestParkResources.Cancel,
                                                                                    "",
                                                                                    //buttons
                                                                                    ContestParkResources.ChooseFromLibrary,
                                                                                    ContestParkResources.TakeAPhoto);
            return await GetPictureStream(selected);
        }

        /// <summary>
        /// <param name="permission"></param> değerine göre izin verilmişmi kontrol eder
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>Erişim izni verilmiş ise true aksi halde false</returns>
        private async Task<bool> CheckPermissionStatusAsync(Permission permission)
        {
            PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);

            if (status != PermissionStatus.Granted)
            {
                Dictionary<Permission, PermissionStatus> results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { permission });
                status = results[permission];
            }

            return status == PermissionStatus.Granted;
        }

        /// <summary>
        /// Galleryden fotoğraf seçtirir
        /// </summary>
        /// <returns></returns>
        private async Task<MediaModel> ChooseFromLibraryAsync()
        {
            bool isPickPhotoSupported = CrossMedia.Current.IsPickPhotoSupported;
            if (!isPickPhotoSupported)
            {
                await ShowErrorMessage();
                return null;
            }

            MediaFile file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                CompressionQuality = 50,
                PhotoSize = PhotoSize.Medium,
                CustomPhotoSize = 90 //Resize to 90% of original
            });

            if (file == null)
                return null;

            return new MediaModel
            {
                File = file.GetStream(),
                FileName = Path.GetFileName(file.Path),
                AnalyticsEventLabel = "Galery"// GA eventleri için kamera mı yoksa galery'den mi fotoğraf yüklediğini ölçmek için eklendi
            };
        }

        /// <summary>
        /// Kamera veya galleriye erişim izni yoksa hata mesajı göster
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        private async Task ShowErrorMessage()
        {
            await _pageDialogService?.DisplayAlertAsync(ContestParkResources.PermissionsDenied,
                                                       ContestParkResources.UnableToTakePhotos,
                                                       ContestParkResources.Okay);
        }

        /// <summary>
        /// Kameradan fotoğraf çeker
        /// </summary>
        /// <returns>Fotoğraf stream</returns>
        private async Task<MediaModel> TakeAPhotoAsync()
        {
            bool isCameraAvailable = CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported;
            if (!isCameraAvailable)
            {
                await ShowErrorMessage();
                return null;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                SaveToAlbum = true,
                CompressionQuality = 50,
                PhotoSize = PhotoSize.Medium,
                CustomPhotoSize = 90 //Resize to 90% of original
            });

            if (file == null)
                return null;

            return new MediaModel
            {
                File = file.GetStream(),
                FileName = Path.GetFileName(file.Path),
                AnalyticsEventLabel = "Kamera"// GA eventleri için kamera mı yoksa galery'den mi fotoğraf yüklediğini ölçmek için eklendi
            };
        }

        #endregion Methods
    }
}
