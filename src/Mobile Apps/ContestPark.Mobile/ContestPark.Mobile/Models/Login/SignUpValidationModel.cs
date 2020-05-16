using System.Collections.Generic;

namespace ContestPark.Mobile.Models.Login
{
    public class SignUpValidationModel
    {
        public List<string> MemberNames { get; set; } = new List<string>();

        public ErrorsModel Errors
        {
            set
            {
                if (value.UserName?.Length > 0)
                {
                    MemberNames.AddRange(value.UserName);
                }

                if (value.FullName?.Length > 0)
                {
                    MemberNames.AddRange(value.FullName);
                }
            }
        }
    }

    public class ErrorsModel
    {
        public string[] UserName { get; set; }
        public string[] FullName { get; set; }
    }
}
