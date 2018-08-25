using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters.Quiz
{
    public class TimeCircleConverter : IValueConverter
    {
        /// <summary>
        /// Value'dan gelen süresi 10 dan çıkarır sürenin yazılması ile cirle kısmının değerlerini eşitlemek için
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 10 - System.Convert.ToInt32(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}