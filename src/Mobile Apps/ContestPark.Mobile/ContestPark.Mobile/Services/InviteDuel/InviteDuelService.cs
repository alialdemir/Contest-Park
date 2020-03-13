using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Views;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Linq;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.InviteDuel
{
    public class InviteDuelService : IInviteDuelService
    {
        #region Private variables

        private readonly IIdentityService _identityService;
        private readonly IPopupNavigation _popupNavigation;

        #endregion Private variables

        #region Constructor

        public InviteDuelService(IIdentityService identityService,
            IPopupNavigation _popupNavigation)
        {
            _identityService = identityService;
            this._popupNavigation = _popupNavigation;
        }

        #endregion Constructor

        #region Methods

        public void InviteDuel()
        {
            int randomSecond = new Random().Next(5000, 20000);

            Device.StartTimer(TimeSpan.FromSeconds(randomSecond), () =>
            {
                string popupName = CurrentPopupName();
                if (popupName == nameof(QuestionExpectedPopupView)
                    || popupName == nameof(QuestionPopupView)
                    || popupName == nameof(AcceptDuelInvitationPopupView))// Eğer düellodaysa davet göndermesin
                    return true;

                return true;
            });
        }

        private string CurrentPopupName()
        {
            if (_popupNavigation == null || _popupNavigation.PopupStack.Count == 0)
                return string.Empty;

            PopupPage popupPage = _popupNavigation.PopupStack.FirstOrDefault();
            if (popupPage == null)
                return string.Empty;

            return popupPage.GetType().Name;
        }

        #endregion Methods
    }
}
