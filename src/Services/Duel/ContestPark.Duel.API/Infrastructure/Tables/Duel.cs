using ContestPark.Core.Database.Models;
using ContestPark.Duel.API.Enums;
using Dapper;

namespace ContestPark.Duel.API.Infrastructure.Tables
{
    [Table("Duels")]
    public class Duel : EntityBase
    {
        [Key]
        public int DuelId { get; set; }

        public string FounderUserId { get; set; }

        public string OpponentUserId { get; set; }

        public short SubCategoryId { get; set; }

        public decimal Bet { get; set; }

        public short ContestDateId { get; set; }

        public BalanceTypes BalanceType { get; set; }

        public DuelTypes DuelType { get; set; }

        public byte FounderTotalScore { get; set; }

        public byte OpponentTotalScore { get; set; }
    }
}
