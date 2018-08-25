using ContestPark.Core.Enums;
using ContestPark.Domain.Duel.Enums;
using System;

namespace ContestPark.Domain.Duel.Model.Request
{
    public class UserAnswer
    {
        public string FounderUserId { get; set; }

        public string OpponentUserId { get; set; }

        public string FounderConnectionId { get; set; }

        public string OpponentConnectionId { get; set; }

        public Languages FounderLanguage { get; set; }

        public Languages OpponentLanguage { get; set; }

        public Int16 SubcategoryId { get; set; }

        public int DuelId { get; set; }

        public bool IsCorrect { get; set; }

        public string ConnectionId { get; set; }

        public Stylish Stylish { get; set; }

        public bool IsFounder { get; set; }

        public byte Time { get; set; }

        public Stylish CorrectAnswer { get; set; }

        public int QuestionInfoId { get; set; }
    }
}