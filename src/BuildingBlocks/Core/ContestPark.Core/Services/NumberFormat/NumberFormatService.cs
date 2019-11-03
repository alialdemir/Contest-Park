namespace ContestPark.Core.Services.NumberFormat
{
    public class NumberFormatService : INumberFormatService
    {
        /// <summary>
        /// Rakamları formatlar örneğin 1k 10k 100m gibi kısaltarak gösterir
        /// </summary>
        /// <param name="number">rakam</param>
        /// <returns></returns>
        public string NumberFormating(long number)
        {
            string number1 = number.ToString();

            if (number1.Length <= 4)
                return number1;
            if (number1.Length == 5)
                return number1.Substring(0, 2) + "K";
            else if (number1.Length >= 6 && number1.Length < 7)
                return number1.Substring(0, 3) + "K";
            else if (number1.Length >= 6)
            {
                if (number1.Substring(1, 1) != "0")
                    return number1.Substring(0, 1) + "," + number1.Substring(1, 1) + "M";
                else if (number1.Length == 8)
                    return number1.Substring(0, 2) + "M";
                else if (number1.Length >= 9)
                    return number1.Substring(0, 3) + "M";
            }

            return "-1";
        }
    }
}
