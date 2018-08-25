using ContestPark.Core.Enums;
using System;

namespace ContestPark.Infrastructure.Signalr.Entities
{
    public class DuelUser
    {
        public string UserId { get; set; }

        public Int16 SubCategoryId { get; set; }

        public int Bet { get; set; }

        public string ConnectionId { get; set; }

        public DateTime Date { get; } = DateTime.Now;

        public Languages Language { get; set; }
    }
}