using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Chat.API.Infrastructure.Tables
{
    [Table("Blocks")]
    public class Block : EntityBaseEffaceable
    {
        [Key]
        public int BlockId { get; set; }

        public string SkirterUserId { get; set; }

        public string DeterredUserId { get; set; }
    }
}
