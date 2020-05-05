using ContestPark.Mobile.Enums;
using Xamarin.Forms;

namespace ContestPark.Mobile.Models.Login
{
    public class SignUpViewModel : BaseModel
    {
        public ImageSource Image { get; set; }
        public string Placeholder { get; set; }
        public bool IsVisibleReferenceCode { get; set; }
        public SignUpTypes SignUpType { get; set; }
    }
}
