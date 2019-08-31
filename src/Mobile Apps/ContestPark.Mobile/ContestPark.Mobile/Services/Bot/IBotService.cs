using System.Windows.Input;

namespace ContestPark.Mobile.Services.Bot
{
    public interface IBotService
    {
        void Init(ICommand saveAnswerCommand, string botUserId);
    }
}
