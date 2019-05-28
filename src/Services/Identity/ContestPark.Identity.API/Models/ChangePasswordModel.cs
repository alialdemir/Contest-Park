namespace ContestPark.Identity.API.Models
{
    public class ChangePasswordModel
    {
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
    }
}