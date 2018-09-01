﻿using ContestPark.Mobile.Enums;
using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Bot
{
    public interface IBotService
    {
        void Init(Func<Stylish, bool, Task> saveAnswer, bool isFounder);
    }
}