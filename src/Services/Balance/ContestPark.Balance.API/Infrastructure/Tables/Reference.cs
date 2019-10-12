using ContestPark.Balance.API.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContestPark.Balance.API.Infrastructure.Tables
{
    public class Reference
    {
        public int ReferenceId { get; set; }

        [MaxLength(256)]
        public string Code { get; set; }

        public BalanceTypes BalanceType { get; set; }

        public decimal Amount { get; set; }

        public int Menstruation { get; set; }

        public DateTime FinishDate { get; set; }
    }
}
