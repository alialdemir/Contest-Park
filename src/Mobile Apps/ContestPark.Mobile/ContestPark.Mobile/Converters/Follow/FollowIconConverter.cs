﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters.Follow
{
    public class FollowIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string svgResourcePath = "resource://ContestPark.Mobile.Common.Images.{0}?assembly=ContestPark.Mobile";
            return (bool)value
                ? string.Format(svgResourcePath, "profile_unfollow.svg")
                : string.Format(svgResourcePath, "profile_follow.svg");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
