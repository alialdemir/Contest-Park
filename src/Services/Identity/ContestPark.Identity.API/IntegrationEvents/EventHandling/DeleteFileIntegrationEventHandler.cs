using ContestPark.EventBus.Abstractions;
using ContestPark.Identity.API.IntegrationEvents.Events;
using ContestPark.Identity.API.Services.BlobStorage;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.IntegrationEvents.EventHandling
{
    public class DeleteFileIntegrationEventHandler :
        IIntegrationEventHandler<DeleteFileIntegrationEvent>
    {
        private readonly IFileUploadService _fileUploadService;

        public DeleteFileIntegrationEventHandler(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// Dosya silme eventi tetiklenirse dosya sil çalışır.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(DeleteFileIntegrationEvent @event)
        {
            await _fileUploadService.DeleteFileAsync(@event.UserId, @event.Uri, @event.PictureType);
        }
    }
}
