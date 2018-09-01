using ContestPark.Core.Enums;
using System;

namespace ContestPark.Domain.Duel.Model.Request
{
    public class GameState
    {
        public string FounderUserId { get; set; }

        public string OpponentUserId { get; set; }

        public string FounderConnectionId { get; set; }

        public string OpponentConnectionId { get; set; }

        public Languages FounderLanguage { get; set; }

        public Languages OpponentLanguage { get; set; }

        public Int16 SubcategoryId { get; set; }

        public int DuelId { get; set; }

        public int Bet { get; set; }
    }
}