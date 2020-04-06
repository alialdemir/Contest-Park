using System;

namespace ContestPark.Mobile.Models.Balance
{
    public class RewardModel
    {
        public TimeSpan NextRewardTime { get; set; }
        public decimal Amount { get; set; }
    }
}
