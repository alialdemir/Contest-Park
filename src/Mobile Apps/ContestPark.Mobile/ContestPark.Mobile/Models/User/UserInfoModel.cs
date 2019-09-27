﻿using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;

namespace ContestPark.Mobile.Models.User
{
    public class UserInfoModel : BaseModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        private string _profilePicturePath = DefaultImages.DefaultProfilePicture;

        public string ProfilePicturePath
        {
            get { return _profilePicturePath; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _profilePicturePath = value;
                }
            }
        }

        private string _coverPicturePath = DefaultImages.DefaultCoverPicture;

        public string CoverPicturePath
        {
            get { return _coverPicturePath; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _coverPicturePath = value;
                }
            }
        }

        public Languages Language { get; set; }

        private bool _isPrivateProfile;

        public bool IsPrivateProfile
        {
            get { return _isPrivateProfile; }
            set
            {
                _isPrivateProfile = value;

                RaisePropertyChanged(() => IsPrivateProfile);
            }
        }
    }
}
