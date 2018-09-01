using ContestPark.Domain.Duel.Enums;
using System;

namespace ContestPark.Domain.Duel.Model.Request
{
    public class UserAnswer
    {
        public Guid Id { get; set; }

        public int DuelId { get; set; }

        public Stylish Stylish { get; set; }

        public Stylish CorrectAnswer { get; set; }

        public bool IsFounder { get; set; }

        public byte Time { get; set; }

        public int QuestionInfoId { get; set; }
    }
}