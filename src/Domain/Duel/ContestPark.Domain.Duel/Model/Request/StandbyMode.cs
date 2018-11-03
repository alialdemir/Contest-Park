using System;

namespace ContestPark.Domain.Duel.Model.Request
{
    public class StandbyMode
    {
        public int Bet { get; set; }

        public string ConnectionId { get; set; }
        public Int16 SubCategoryId { get; set; }
    }
}