using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Models.User;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
        bool IsSoundEffectActive { get; }

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

        #endregion Properties

        #region Methods

        /// <summary>
        /// The AddOrUpdateValue
        /// </summary>
        /// <param name="value">The value <see cref="bool"/></param>
        /// <param name="methodName">The methodName <see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task AddOrUpdateValue(bool value, [CallerMemberName]string methodName = "");

        /// <summary>
        /// The AddOrUpdateValue
        /// </summary>
        /// <param name="value">The value <see cref="byte"/></param>
        /// <param name="methodName">The methodName <see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task AddOrUpdateValue(byte value, [CallerMemberName]string methodName = "");

        /// <summary>
        /// The AddOrUpdateValue
        /// </summary>
        /// <param name="value">The value <see cref="string"/></param>
        /// <param name="methodName">The methodName <see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task AddOrUpdateValue(string value, [CallerMemberName]string methodName = "");
        Task<UserInfoModel> GetUserInfo();

        /// <summary>
        /// The GetValueOrDefault
        /// </summary>
        /// <param name="defaultValue">The defaultValue <see cref="bool"/></param>
        /// <param name="methodName">The methodName <see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        bool GetValueOrDefault(bool defaultValue, [CallerMemberName]string methodName = "");

        /// <summary>
        /// The GetValueOrDefault
        /// </summary>
        /// <param name="defaultValue">The defaultValue <see cref="byte"/></param>
        /// <param name="methodName">The methodName <see cref="string"/></param>
        /// <returns>The <see cref="int"/></returns>
        byte GetValueOrDefault(byte defaultValue, [CallerMemberName]string methodName = "");

        /// <summary>
        /// The GetValueOrDefault
        /// </summary>
        /// <param name="defaultValue">The defaultValue <see cref="string"/></param>
        /// <param name="methodName">The methodName <see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        string GetValueOrDefault(string defaultValue, [CallerMemberName]string methodName = "");

        void RefreshCurrentUser(UserInfoModel currentUser);

        /// <summary>
        /// The RemoveCurrentUser
        /// </summary>
        void RemoveCurrentUser();

        void SetTokenInfo(UserToken userToken);

        #endregion Methods
    }
}
