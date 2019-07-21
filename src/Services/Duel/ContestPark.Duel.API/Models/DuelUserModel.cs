using ContestPark.Duel.API.Enums;
using System;

namespace ContestPark.Duel.API.Models
{
    [Serializable]
    public class DuelUserModel
    {
        public string UserId { get; set; }

        public short SubCategoryId { get; set; }

        public decimal Bet { get; set; }

        public string ConnectionId { get; set; }

        public BalanceTypes BalanceType { get; set; }
    }
}
