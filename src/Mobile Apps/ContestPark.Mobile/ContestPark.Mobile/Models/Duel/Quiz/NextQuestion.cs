using ContestPark.Mobile.Enums;

namespace ContestPark.Mobile.Models.Duel.Quiz
{
    public class NextQuestion
    {
        public Stylish FounderStylish { get; set; }

        public Stylish OpponentStylish { get; set; }

        public Stylish CorrectStylish { get; set; }

        public byte FounderScore { get; set; }

        public byte OpponentScore { get; set; }
        public byte Round { get; set; }
        public bool IsGameEnd { get; set; }
    }
}
