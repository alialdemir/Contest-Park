namespace ContestPark.Duel.API.Enums
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
        /// Düello beabere
        /// </summary>
        Draw = 3,

        /// <summary>
        /// In app purchase satın alım yaptı
        /// </summary>
        Buy = 4,

        /// <summary>
        /// Alt kategori kilidini açtı
        /// </summary>
        UnLockSubCategory = 5,

        /// <summary>
        /// Günlük login olma altın hakkını aldı
        /// </summary>
        DailyChip = 6,

        /// <summary>
        /// Görev yaptı
        /// </summary>
        Mission = 7,

        /// <summary>
        /// Joker alımı yaptı
        /// </summary>
        Boost = 8,

        /// <summary>
        /// Düello başladığı anda bahis miktarı kadar bakiye azaltmak için
        /// </summary>
        Duel = 9,
    }
}
