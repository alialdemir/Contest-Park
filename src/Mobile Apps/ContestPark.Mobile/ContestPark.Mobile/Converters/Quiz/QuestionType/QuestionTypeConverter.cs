using ContestPark.Mobile.Enums;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters.Quiz.QuestionType
{
    public class QuestionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((QuestionTypes)value) == QuestionTypes.Image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
