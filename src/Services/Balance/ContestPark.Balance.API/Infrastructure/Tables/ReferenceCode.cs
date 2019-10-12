using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Balance.API.Infrastructure.Tables
{
    [Table("ReferenceCodes")]
    public class ReferenceCode : EntityBase
    {
        [Key]
        public int ReferenceCodeId { get; set; }

        public string ReferenceUserId { get; set; }

        public string NewUserId { get; set; }

        public string Code { get; set; }
    }
}
