using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ContestPark.Identity.API.Validations
{
    public class NotTurkishCharacterAttribute : ValidationAttribute
    {
        /// <summary>
        /// Yazının içinde türkçe karakter var mı kontrol eder
        /// </summary>
        /// <param name="value">Herhangi bir yazı</param>
        /// <returns>Türkçe harf varsa true yoksa false</returns>
        public override bool IsValid(object value)
        {
            if (!(value is string))
                return true;

            string newVaşue = value.ToString().Replace("\n", "").Trim().ToLower();//sondaki boşluğu sildik ve büyük harf yaptık

            //türkçe harfler
            char[] turkishCharaeters = new char[] { 'ö', 'Ö', 'ü', 'Ü', 'ç', 'Ç', 'İ', 'ı', 'Ğ', 'ğ', 'Ş', 'ş' };

            return !turkishCharaeters.Intersect(newVaşue).Any();
        }
    }
}