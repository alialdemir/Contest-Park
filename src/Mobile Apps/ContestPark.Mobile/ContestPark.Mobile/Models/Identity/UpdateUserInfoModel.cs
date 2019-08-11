namespace ContestPark.Mobile.Models.Identity
{
    public class UpdateUserInfoModel
    {
        public string FullName { get; set; }

        public string UserName { get; set; }

        public bool? IsPrivateProfile { get; set; }
    }
}
