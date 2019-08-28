using ContestPark.EventBus.Events;

namespace ContestPark.Identity.API.IntegrationEvents.Events
{
    public class ChangeBalanceIntegrationEvent : IntegrationEvent
    {
        public decimal Amount { get; set; }
        public string UserId { get; set; }

        public BalanceTypes BalanceType { get; set; }
        public BalanceHistoryTypes BalanceHistoryType { get; set; }

        public ChangeBalanceIntegrationEvent(decimal amount, string userId, BalanceTypes balanceType, BalanceHistoryTypes balanceHistoryType)
        {
            Amount = amount;
            UserId = userId;
            BalanceType = balanceType;
            BalanceHistoryType = balanceHistoryType;
        }
    }

    public enum BalanceTypes : byte
    {
        /// <summary>
        /// Oyun parasındaki bakiye
        /// </summary>
        Gold = 1,

        /// <summary>
        /// Gerçek para bakiyesi..
        /// </summary>
        Money = 2
    }

    public enum BalanceHistoryTypes : byte
    {
        /// <summary>
        /// Düello kazandı
        /// </summary>
        Win = 1,

        /// <summary>
        /// Düello yenildi
        /// </summary>
        Defeat = 2,

        /// <summary>
        /// In app purchase satın alım yaptı
        /// </summary>
        Buy = 3,

        /// <summary>
        /// Alt kategori kilidini açtı
        /// </summary>
        UnLockSubCategory = 4,

        /// <summary>
        /// Günlük login olma altın hakkını aldı
        /// </summary>
        DailyChip = 5,

        /// <summary>
        /// Görev yaptı
        /// </summary>
        Mission = 6,

        /// <summary>
        /// Joker alımı yaptı
        /// </summary>
        Boost = 7
    }
}
