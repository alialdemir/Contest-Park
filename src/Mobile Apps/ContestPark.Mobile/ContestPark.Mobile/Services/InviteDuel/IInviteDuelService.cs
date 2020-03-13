using System.Windows.Input;

namespace ContestPark.Mobile.Services.InviteDuel
{
    public interface IInviteDuelService
    {
        ICommand InviteDuelCommand { get; }
    }
}
