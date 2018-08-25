using ContestPark.Core.Enums;
using System;

namespace ContestPark.Domain.Signalr.Model.Request
{
    public class WaitingOpponent
    {
        public int Bet { get; set; }

        public Int16 SubCategoryId { get; set; }

        public string UserId { get; set; }

        public string ConnectionId { get; set; }

        public Languages Language { get; set; }

        public WaitingOpponent(string userId,
            string connectionId,
            Int16 subCategoryId,
            int bet,
            Languages language)
        {
            UserId = userId;
            ConnectionId = connectionId;
            SubCategoryId = subCategoryId;
            Bet = bet;
            Language = language;
        }
    }
}