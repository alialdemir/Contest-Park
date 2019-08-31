using System;

namespace ContestPark.Duel.API.Services.ScoreCalculator
{
    public class ScoreCalculator : IScoreCalculator
    {
        /// <summary>
        /// Skor hesaplama makismum tüm soruları 10. saniyede cevaplasa bile alabileceği maksimum skor 140
        /// r = kaçıncı soruda olduğu
        /// t = kaçıncı saniyede soruya cevap verdiği
        /// formül:  (r * t) / 2
        /// </summary>
        /// <param name="round">Kaçıncı soruda(raund) da olduğu</param>
        /// <param name="time">kaçıncı saniyede soruya cevap verdiği</param>
        /// <returns>Skor</returns>
        public byte Calculator(int round, byte time)
        {
            if (time <= 0 || time > 10)
                time = 1;

            if (round <= 0 || round > 7)
                round = 1;

            double score = Math.Round((double)(round * time / 2));// maksimum 140 gelir

            return Convert.ToByte(score);
        }
    }
}
