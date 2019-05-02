﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters.CategoryDetail
{
    public class FollowButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "fas-heart" : "far-heart";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}