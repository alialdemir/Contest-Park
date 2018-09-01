using ContestPark.Mobile.Enums;
using System;

namespace ContestPark.Mobile.Models.Duel.Quiz
{
    public class NextQuestion
    {
        public Guid Id { get; set; }

        public Question Question { get; set; }

        public Stylish FounderStylish { get; set; }

        public Stylish OpponentStylish { get; set; }

        public byte FounderScore { get; set; }

        public byte OpponentScore { get; set; }

        public bool IsGameEnd { get; set; }
    }
}