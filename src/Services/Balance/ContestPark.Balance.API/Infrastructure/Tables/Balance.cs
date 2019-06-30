using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Balance.API.Infrastructure.Tables
{
    [Table("Balances")]
    public class Balance : EntityBase
    {
        [Key]
        public string UserId { get; set; }

        public decimal Gold { get; set; }
        public decimal Money { get; set; }
    }
}
