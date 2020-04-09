namespace ContestPark.Duel.API.Models
{
    public class DuelWinStatusModel
    {
        /// <summary>
        /// Para miktarı 70 TL'den fazla ise true döner ve oyuncu yenilmeli
        /// </summary>
        public bool Check1 { get; set; }

        /// <summary>
        /// Altın miktarı 1000 altın'dan az ise ve altınla düello yapılıyorsa oyuncu yenmeli
        /// </summary>
        public bool Check2 { get; set; }

        /// <summary>
        /// Kaybettiği düello sayısı kazandığı düello sayısından büyük ise true döner
        /// True dönerse oyuncu yenmeli
        /// </summary>
        public bool Check3 { get; set; }

        /// <summary>
        /// Bakiyesi iki defa 70 tl geçmişse true döner ve yenilmesi gerekir ama oynadığı düello para ve  Check2 false ise
        /// </summary>
        public bool Check4 { get; set; }

        /// <summary>
        /// Kazandığı düello sayısı kaybettiğiığı düello sayısından büyük ise true döner
        /// True dönerse oyuncu yenilmeli
        /// </summary>
        public bool Check5 { get; set; }
    }
}
