namespace ContestPark.Duel.API.Models
{
    public class DuelWinStatusModel
    {
        /// <summary>
        /// Para miktarı 70 TL'den fazla ise true döner ve oyuncu yenilmeli
        /// </summary>
        public bool Status1 { get; set; }

        /// <summary>
        /// Altın miktarı 1000 altın'dan az ise ve altınla düello yapılıyorsa oyuncu yenmeli
        /// </summary>
        public bool Status2 { get; set; }

        /// <summary>
        /// Kaybettiği düello sayısı kazandığı düello sayısından büyük ise true döner
        /// True dönerse oyuncu yenmeli
        /// </summary>
        public bool Status3 { get; set; }

        /// <summary>
        /// Bakiyesi iki defa 70 tl geçmişse true döner ve yenilmesi gerekir ama oynadığı düello para ve Status2 false ise
        /// </summary>
        public bool Status4 { get; set; }
    }
}
