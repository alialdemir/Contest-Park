using System;

namespace ContestPark.Domain.Signalr.Model.Request
{
    public class WaitingOpponentRemove
    {
        public int Bet { get; set; }

        public Int16 SubCategoryId { get; set; }

        public string UserId { get; set; }

        public string ConnectionId { get; set; }

        public WaitingOpponentRemove(string userId,
            string connectionId,
            Int16 subCategoryId,
            int bet)
        {
            UserId = userId;
            ConnectionId = connectionId;
            SubCategoryId = subCategoryId;
            Bet = bet;
        }
    }
}