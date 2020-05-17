using ContestPark.Mobile.Models.Duel.Bet;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Models.User;
using System.Collections.Generic;

namespace ContestPark.Mobile.Services.Settings
{
    /// <summary>
    /// Defines the <see cref="ISettingsService"/>
    /// </summary>
    public interface ISettingsService
    {
        #region Properties

        /// <summary>
        /// Gets or sets the AuthAccessToken
        /// </summary>
        string AuthAccessToken { get; set; }

        /// <summary>
        /// Gets the CurrentUser
        /// </summary>
        UserInfoModel CurrentUser { get; }

        /// <summary>
        /// Gets a value indicating whether IsSoundEffectActive
        /// </summary>
        bool IsSoundEffectActive { get; set; }

        /// <summary>
        /// Tamamlanmamış düello id ekle
        /// </summary>
        /// <param name="duelId">Düello id</param>
        void AddPendingDuelId(int duelId);

        /// <summary>
        /// Tamamlanmamış bekleyen düello id varsa onu siler
        /// </summary>
        /// <param name="duelId">Düello id</param>
        void RemovePendingDuelId(int duelId);

        /// <summary>
        /// Tamamlanmamış düello id'leri
        /// </summary>
        /// <returns>Tamamlanmayan düello id'leri</returns>
        List<int> GetPendingDuelIds();

        /// <summary>
        /// Gets or sets the RefleshToken
        /// </summary>
        string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the SignalRConnectionId
        /// </summary>
        string SignalRConnectionId { get; set; }

        /// <summary>
        /// Üye olma sayısı
        /// </summary>
        byte SignUpCount { get; set; }

        /// <summary>
        /// Gets or sets the TokenType
        /// </summary>
        string TokenType { get; set; }

        bool IsTutorialDisplayed { get; set; }

        string LastUpdatedScopeName { get; set; }

        /// <summary>
        /// En son oynanan bakiye tipi
        /// </summary>
        BetModel LastSelectedBet { get; set; }

        #endregion Properties

        #region Methods

        void RefreshCurrentUser(UserInfoModel currentUser);

        /// <summary>
        /// The RemoveCurrentUser
        /// </summary>
        void RemoveCurrentUser();

        void SetTokenInfo(UserToken userToken);

        #endregion Methods
    }
}
