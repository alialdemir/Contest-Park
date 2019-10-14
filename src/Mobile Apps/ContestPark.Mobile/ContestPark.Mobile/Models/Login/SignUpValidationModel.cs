using Newtonsoft.Json;
using System.Collections.Generic;

namespace ContestPark.Mobile.Models.Login
{
    public class SignUpValidationModel
    {
        public string[] UserName { get; set; }
        public string[] FullName { get; set; }

        private List<string> _errors = new List<string>();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Errors
        {
            get
            {
                if (UserName?.Length > 0)
                {
                    _errors.AddRange(UserName);
                }

                if (FullName?.Length > 0)
                {
                    _errors.AddRange(FullName);
                }

                return _errors.ToArray();
            }
        }
    }
}
