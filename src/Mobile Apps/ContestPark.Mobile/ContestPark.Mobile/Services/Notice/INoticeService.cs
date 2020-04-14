using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Models.Slide;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Notice
{
    public interface INoticeService
    {
        Task<ServiceModel<NoticeModel>> NoticesAsync(PagingModel pagingModel);
    }
}
