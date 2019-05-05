﻿namespace ContestPark.Mobile.Models.Follow
{
    public class FollowModel : BaseModel
    {
        private bool isFollowing;
        public string FullName { get; set; }

        public bool IsFollowing
        {
            get { return isFollowing; }
            set
            {
                isFollowing = value;
                RaisePropertyChanged(() => IsFollowing);
            }
        }

        public string ProfilePicturePath { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}