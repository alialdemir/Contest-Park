namespace ContestPark.Balance.API.Enums
{
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