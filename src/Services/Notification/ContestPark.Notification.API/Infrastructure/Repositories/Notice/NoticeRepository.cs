using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Notification.API.Models;

namespace ContestPark.Notification.API.Infrastructure.Repositories.Notice
{
    public class NoticeRepository : INoticeRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Notice> _noticeRepository;

        #endregion Private Variables

        #region Constructor

        public NoticeRepository(IRepository<Tables.Notice> noticeRepository)
        {
            _noticeRepository = noticeRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Aktif duyuruları döndürür
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Duyuru listesi</returns>
        public ServiceModel<NoticeModel> Notices(PagingModel pagingModel)
        {
            string sql = @"SELECT n.PicturePath, n.Link FROM
                           WHERE n.IsActive = 1";

            return _noticeRepository.ToServiceModel<NoticeModel>(sql, pagingModel: pagingModel);
        }

        #endregion Methods
    }
}
