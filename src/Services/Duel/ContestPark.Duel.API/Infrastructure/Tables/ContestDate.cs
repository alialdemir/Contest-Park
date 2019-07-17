using ContestPark.Core.Database.Models;
using Dapper;
using System;

namespace ContestPark.Duel.API.Infrastructure.Tables
{
    [Table("ContestDates")]
    public class ContestDate : EntityBase
    {
        [Key]
        public short ContestDateId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }
    }
}
