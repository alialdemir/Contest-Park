namespace ContestPark.Identity.API.Models
{
    public class ProfileInfoModel
    {
        public string CoverPicture { get; set; }

        public long FollowersCount { get; set; }

        public long FollowUpCount { get; set; }

        public string FullName { get; set; }

        public long GameCount { get; set; }

        public bool? IsBlocked { get; set; }

        public bool? IsFollowing { get; set; }

        public string ProfilePicturePath { get; set; }

        public string UserId { get; set; }
    }
}
