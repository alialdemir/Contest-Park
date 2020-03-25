using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Duel.Quiz;
using System;
using System.Windows.Input;
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
        public void Init(ICommand saveAnswerCommand, string botUserId, BalanceTypes balanceType)
        {
            Random rnd = new Random();
            Stylish ramdomStylish = (Stylish)rnd.Next(1, 4);
            int randomSecound = rnd.Next(1, 5);

            if (balanceType == BalanceTypes.Money)
            {
                saveAnswerCommand.Execute(new SaveAnswerModel
                {
                    UserId = botUserId,
                    Stylish = ramdomStylish,
                });

                return;
            }

            Device.StartTimer(new TimeSpan(0, 0, 0, randomSecound, 0), () =>
            {
                saveAnswerCommand.Execute(new SaveAnswerModel
                {
                    UserId = botUserId,
                    Stylish = ramdomStylish,
                });

                return false;
            });
        }

        #endregion Methods
    }
}
