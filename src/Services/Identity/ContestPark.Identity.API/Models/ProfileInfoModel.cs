namespace ContestPark.Identity.API.Models
{
    public class ProfileInfoModel
    {
        public string CoverPicture { get; set; }

        public string FollowersCount { get; set; }

        public string FollowUpCount { get; set; }

        public string FullName { get; set; }

        public string GameCount { get; set; }

        public bool? IsBlocked { get; set; }

        public bool? IsFollowing { get; set; }

        public string ProfilePicturePath { get; set; }

        public string UserId { get; set; }
        public bool IsPrivateProfile { get; set; }
    }
}
