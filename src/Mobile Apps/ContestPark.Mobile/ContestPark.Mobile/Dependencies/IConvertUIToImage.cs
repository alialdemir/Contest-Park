﻿using Xamarin.Forms;

namespace ContestPark.Mobile.Dependencies
{
    public interface IConvertUIToImage
    {
        string GetImagePathByPage(ContentPage contentPage);
    }
}