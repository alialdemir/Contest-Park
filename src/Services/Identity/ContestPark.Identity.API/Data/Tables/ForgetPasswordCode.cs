namespace ContestPark.Identity.API.Data.Tables
{
    public class ForgetPasswordCode
    {
        public int Code { get; set; }
        public int ForgetPasswordCodeId { get; set; }
        public string UserId { get; set; }
    }
}