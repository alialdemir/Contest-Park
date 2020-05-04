using ContestPark.Mobile.Enums;

namespace ContestPark.Mobile.Models.Duel.Bet
{
    public class BetModel : BaseModel
    {
        public decimal EntryFee { get; set; }
        public decimal Prize { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public BalanceTypes BalanceType { get; set; }
        public int CurrentIndex { get; set; }
    }
}
