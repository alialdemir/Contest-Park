using ContestPark.Duel.API.Enums;

namespace ContestPark.Duel.API.Models
{
    public class DuelResultModel
    {
        public string FounderFullName { get; set; }

        public string FounderProfilePicturePath { get; set; }

        public string FounderUserId { get; set; }

        public string FounderUserName { get; set; }

        public decimal Gold { get; set; }

        public bool IsFounder { get; set; }

        public string OpponentFullName { get; set; }

        public string OpponentProfilePicturePath { get; set; }

        public string OpponentUserId { get; set; }

        public string OpponentUserName { get; set; }

        public BalanceTypes BalanceType { get; set; }

        public short SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public short FounderLevel { get; set; } = 1;

        public short OpponentLevel { get; set; } = 1;

        public byte FounderScore { get; set; }

        public byte OpponentScore { get; set; }

        public string SubCategoryPicturePath { get; set; }

        private byte? _finishBonus;

        public byte? FinishBonus
        {
            get { return _finishBonus ?? 0; }
            set
            {
                _finishBonus = value;
            }
        }

        private byte? _victoryBonus;

        public byte? VictoryBonus
        {
            get { return _victoryBonus ?? 0; }
            set
            {
                _victoryBonus = value;
            }
        }
    }
}
