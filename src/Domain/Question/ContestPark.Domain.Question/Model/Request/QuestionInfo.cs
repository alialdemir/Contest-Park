using ContestPark.Core.Enums;
using System;

namespace ContestPark.Domain.Question.Model.Request
{
    public class QuestionInfo
    {
        public Int16 SubCategoryId { get; set; }

        public string FounderUserId { get; set; }

        public string OpponentUserId { get; set; }

        public string FounderConnectionId { get; set; }

        public string OpponentConnectionId { get; set; }

        public Languages FounderLanguage { get; set; }

        public Languages OpponentLanguage { get; set; }

        public QuestionInfo(Int16 subCategoryId,
            string founderUserId,
            string opponentUserId,
            string founderConnectionId,
            string opponentConnectionId,
            Languages founderLanguage,
            Languages opponentLanguage)
        {
            SubCategoryId = subCategoryId;
            FounderUserId = founderUserId;
            OpponentUserId = opponentUserId;
            FounderConnectionId = founderConnectionId;
            OpponentUserId = opponentUserId;
            OpponentConnectionId = opponentConnectionId;
            FounderLanguage = founderLanguage;
            OpponentLanguage = opponentLanguage;
        }
    }
}