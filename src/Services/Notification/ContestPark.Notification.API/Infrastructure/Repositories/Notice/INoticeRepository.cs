using ContestPark.Core.Database.Models;
using ContestPark.Notification.API.Models;

namespace ContestPark.Notification.API.Infrastructure.Repositories.Notice
{
    public interface INoticeRepository
    {
        ServiceModel<NoticeModel> Notices(PagingModel pagingModel);
    }
}
