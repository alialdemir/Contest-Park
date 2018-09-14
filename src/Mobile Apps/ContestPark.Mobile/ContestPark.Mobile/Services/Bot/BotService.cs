using ContestPark.Mobile.Enums;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.Bot
{
    public class BotService : IBotService
    {
        #region Methods

        /// <summary>
        /// Oyuna botu ekler
        /// </summary>
        /// <param name="saveAnswer">Cevap verince göndereceği method</param>
        /// <param name="isFounder">Kullanıcı oyunda kurucu mu yoksa rakip mi</param>
        public void Init(Func<Stylish, bool, Task> saveAnswer, bool isFounder)
        {
            Random rnd = new Random();
            Stylish ramdomStylish = (Stylish)rnd.Next(0, 3);
            int randomSecound = rnd.Next(1, 5);

            Device.StartTimer(new TimeSpan(0, 0, 0, randomSecound, 0), () =>
            {
                saveAnswer?.Invoke(ramdomStylish, !isFounder);

                return false;
            });
        }

        #endregion Methods
    }
}