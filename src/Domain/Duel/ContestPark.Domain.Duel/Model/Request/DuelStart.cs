using ContestPark.Core.Enums;
using System;

namespace ContestPark.Domain.Duel.Model.Request
{
    public class DuelStart
    {
        public DuelStart(Int16 subCategoryId,
            int bet,
            string founderUserId,
            string opponentUserId,
            string founderConnectionId,
            string opponentConnectionId,
            Languages founderLanguage,
            Languages opponentLanguage
        )
        {
            SubCategoryId = subCategoryId;
            Bet = bet;
            FounderUserId = founderUserId;
            OpponentUserId = opponentUserId;
            FounderConnectionId = founderConnectionId;
            OpponentConnectionId = opponentConnectionId;
            FounderLanguage = founderLanguage;
            OpponentLanguage = opponentLanguage;
            OpponentUserId = opponentUserId;
        }

        public int Bet { get; set; }
        public string FounderConnectionId { get; set; }
        public Languages FounderLanguage { get; set; }
        public string FounderUserId { get; set; }
        public string OpponentConnectionId { get; set; }
        public Languages OpponentLanguage { get; set; }
        public string OpponentUserId { get; set; }
        public Int16 SubCategoryId { get; set; }
    }
}