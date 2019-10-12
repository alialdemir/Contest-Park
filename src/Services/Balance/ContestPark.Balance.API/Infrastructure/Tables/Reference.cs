using ContestPark.Balance.API.Enums;
using ContestPark.Core.Database.Models;
using Dapper;
using System;

namespace ContestPark.Balance.API.Infrastructure.Tables
{
    [Table("References")]
    public class Reference : EntityBase
    {
        [Key]
        public int ReferenceId { get; set; }

        public string Code { get; set; }

        public BalanceTypes BalanceType { get; set; }

        public decimal Amount { get; set; }

        public int Menstruation { get; set; }

        public DateTime FinishDate { get; set; }
    }
}
