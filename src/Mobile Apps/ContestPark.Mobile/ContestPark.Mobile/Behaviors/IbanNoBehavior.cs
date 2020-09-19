using Xamarin.Forms;

namespace ContestPark.Mobile.Behaviors
{
    public class IbanNoBehavior : MaskedBehavior
    {
        protected override void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as Entry;

            var text = entry.Text.ToUpper();

            if (text.StartsWith("TR"))
            {
                entry.Text = text.Replace("TR", "");
                return;
            }

            base.OnEntryTextChanged(sender, args);
        }
    }
}
