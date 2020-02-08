using ContestPark.EventBus.Abstractions;
using ContestPark.Identity.API.IntegrationEvents.Events;
using ContestPark.Identity.API.Services.BlobStorage;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.IntegrationEvents.EventHandling
{
    public class DeleteFileIntegrationEventHandler :
        IIntegrationEventHandler<DeleteFileIntegrationEvent>
    {
        #region Private variables

        private readonly IFileUploadService _fileUploadService;

        #endregion Private variables

        #region Constructor

        public DeleteFileIntegrationEventHandler(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Dosya silme eventi tetiklenirse dosya sil çalışır.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task Handle(DeleteFileIntegrationEvent @event)
        {
            _fileUploadService.DeleteFile(@event.UserId, @event.Uri, @event.PictureType);

            return Task.CompletedTask;
        }

        #endregion Methods
    }
}
