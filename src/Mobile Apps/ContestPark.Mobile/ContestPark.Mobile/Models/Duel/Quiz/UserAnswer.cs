using ContestPark.Mobile.Enums;
using System;

namespace ContestPark.Mobile.Models.Duel.Quiz
{
    public class UserAnswer
    {
        public Guid Id { get; set; }

        public int DuelId { get; set; }

        public Stylish CorrectAnswer { get; set; }

        public bool IsFounder { get; set; }

        public Stylish Stylish { get; set; }

        public byte Time { get; set; }

        public int QuestionInfoId { get; set; }
    }
}