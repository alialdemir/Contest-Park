using ContestPark.Core.Database.Models;
using ContestPark.Notification.API.Infrastructure.Repositories.Notice;
using ContestPark.Notification.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace ContestPark.Notification.API.Controllers
{
    public class NoticeController : Core.Controllers.ControllerBase
    {
        #region Private variables

        private readonly INoticeRepository _noticeRepository;

        #endregion Private variables

        #region Constructor

        public NoticeController(ILogger<NoticeController> logger,
                                INoticeRepository noticeRepository) : base(logger)
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
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ServiceModel<NoticeModel>), (int)HttpStatusCode.OK)]
        public IActionResult Notice(PagingModel pagingModel)
        {
            ServiceModel<NoticeModel> notices = _noticeRepository.Notices(pagingModel);
            if (notices.Items == null || !notices.Items.Any())
                return NotFound();

            return Ok(notices);
        }

        #endregion Methods
    }
}
