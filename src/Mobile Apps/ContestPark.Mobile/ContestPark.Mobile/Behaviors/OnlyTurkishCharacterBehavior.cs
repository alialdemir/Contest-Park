using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ContestPark.Mobile.Behaviors
{
    public class OnlyTurkishCharacterBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private Dictionary<char, char> Characters = new Dictionary<char, char>
        {
            {'Ç', 'C'},
            {'Ö', 'O'},
            {'Ş', 'S'},
            {'İ', 'I'},
            {'I', 'i'},
            {'Ü', 'U'},
            {'Ğ', 'G'},
            {'ç', 'c'},
            {'ö', 'o'},
            {'ş', 's'},
            {'ı', 'i'},
            {'ü', 'u'},
            {'ğ', 'g'},
        };

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as Entry;

            var text = entry.Text;

            if (string.IsNullOrWhiteSpace(text))
                return;

            char lastCharacter = Convert.ToChar(text.Substring(text.Length - 1, 1));
            if (Characters.ContainsKey(lastCharacter))
                text = text.Replace(lastCharacter, Characters[lastCharacter]);

            if (entry.Text != text)
                entry.Text = text;
        }
    }
}
