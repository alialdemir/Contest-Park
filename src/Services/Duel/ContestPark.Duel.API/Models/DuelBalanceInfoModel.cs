﻿using ContestPark.Duel.API.Enums;

namespace ContestPark.Duel.API.Models
{
    public class DuelBalanceInfoModel
    {
        public short SubCategoryId { get; set; }
        public decimal Bet { get; set; }
        public BalanceTypes BalanceType { get; set; }

        public byte BetCommission { get; set; }
    }
}
