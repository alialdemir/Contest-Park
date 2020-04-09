using System.Collections.Generic;

namespace ContestPark.Mobile.Models.Login
{
    public class SignUpModel
    {
        public string FullName { get; set; }

        public string LanguageCode { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }

        public string ReferenceCode { get; set; }

        public string DeviceIdentifier { get; set; }

        public List<short> SubCategories { get; set; } = new List<short>();
    }
}
