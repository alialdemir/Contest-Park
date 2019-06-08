namespace ContestPark.Follow.API.Models
{
    public class FollowModel
    {
        public string FullName { get; set; }

        public bool IsFollowing { get; set; }

        public string ProfilePicturePath { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}