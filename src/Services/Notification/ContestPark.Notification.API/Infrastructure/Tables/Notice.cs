using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using Dapper;

namespace ContestPark.Notification.API.Infrastructure.Tables
{
    [Table("Notices")]
    public class Notice : EntityBase
    {
        [Key]
        public short NoticeId { get; set; }

        public Languages Language { get; set; }

        public string PicturePath { get; set; }

        public string Link { get; set; }

        public bool IsActice { get; set; }
    }
}
