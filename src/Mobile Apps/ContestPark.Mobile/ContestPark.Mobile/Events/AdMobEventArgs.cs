using System;

namespace ContestPark.Mobile.Events
{
    public class AdMobEventArgs : EventArgs
    {
        public int? ErrorCode;
        public int RewardAmount;
        public string RewardType;
    }
}
