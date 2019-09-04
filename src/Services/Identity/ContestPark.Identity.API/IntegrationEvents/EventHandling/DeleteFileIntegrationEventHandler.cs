using ContestPark.EventBus.Abstractions;
using ContestPark.Identity.API.IntegrationEvents.Events;
using ContestPark.Identity.API.Services.BlobStorage;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.IntegrationEvents.EventHandling
{
    public class DeleteFileIntegrationEventHandler :
        IIntegrationEventHandler<DeleteFileIntegrationEvent>
    {
        private readonly IFileUploadService _blobStorageService;

        public DeleteFileIntegrationEventHandler(IFileUploadService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        /// <summary>
        /// Dosya silme eventi tetiklenirse dosya sil çalışır.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(DeleteFileIntegrationEvent @event)
        {
            await _blobStorageService.DeleteFileAsync(@event.UserId, @event.Uri, @event.PictureType);
        }
    }
}
