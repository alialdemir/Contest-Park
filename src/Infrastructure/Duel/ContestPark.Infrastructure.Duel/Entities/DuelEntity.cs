using ContestPark.Core.Model;
using System;

namespace ContestPark.Infrastructure.Duel.Entities
{
    public class DuelEntity : EntityBase
    {
        public int DuelId { get; set; }

        public string FounderUserId { get; set; }

        public string OpponentUserId { get; set; }

        public Int16 SubCategoryId { get; set; }

        public int Bet { get; set; }

        public byte FounderTotalScore { get; set; }

        public byte OpponentTotalScore { get; set; }
    }
}