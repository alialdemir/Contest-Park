using ContestPark.Mobile.Enums;

namespace ContestPark.Mobile.Models.ErrorModel
{
    public class ValidationResultModel
    {
        public string ErrorMessage { get; set; }

        public string[] MemberNames { get; set; }
        public ErrorStatuCodes ErrorStatuCode { get; set; }
    }
}
