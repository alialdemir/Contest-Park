using ContestPark.Mobile.Components;
using Xamarin.Forms;

namespace ContestPark.Mobile.Views
{
    public partial class SignUpVerificationView : ContentPage
    {
        #region Constructor

        public SignUpVerificationView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Methods

        private void Code_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            CustomEntry entry = (CustomEntry)sender;
            if (string.IsNullOrEmpty(entry.Text))
            {
                if (sender.Equals(code4))
                {
                    code3.Focus();
                }
                else if (sender.Equals(code3))
                {
                    code2.Focus();
                }
                else if (sender.Equals(code2))
                {
                    code1.Focus();
                }
                else
                    return;
            }
            else if (sender.Equals(code1))
            {
                code2.Focus();
            }
            else if (sender.Equals(code2))
            {
                code3.Focus();
            }
            else if (sender.Equals(code3))
            {
                code4.Focus();
            }
        }

        #endregion Methods
    }
}
