using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Duel.API.Infrastructure.Tables
{
    [Table("ScoreRankings")]
    public class ScoreRanking : EntityBase
    {
        [Key]
        public int ScoreRankingId { get; set; }

        public short ContestDateId { get; set; }

        public string UserId { get; set; }

        public short SubCategoryId { get; set; }

        public int TotalMoneyScore { get; set; }

        public int TotalGoldScore { get; set; }

        public string DisplayTotalMoneyScore { get; set; }

        public string DisplayTotalGoldScore { get; set; }
    }
}
