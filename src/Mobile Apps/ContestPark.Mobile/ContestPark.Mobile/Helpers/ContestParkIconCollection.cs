using Plugin.Iconize;
using System.Collections.Generic;

namespace ContestPark.Mobile.Helpers
{
    public class ContestParkIconCollection
    {
        /// <summary>

        /// Gets the SolidIcons.
        /// </summary>
        /// <value>The SolidIcons.</value>
        public static IList<IIcon> SolidIcons { get; } = new List<IIcon>();

        /// <summary>
        /// Initializes the <see cref="FontAwesomeCollection" /> class.
        /// </summary>
        static ContestParkIconCollection()
        {
            #region Solid

            SolidIcons.Add("cp-chat", '\u0041');
            SolidIcons.Add("cp-comment", '\u0042');
            SolidIcons.Add("cp-contestcoin", '\u0043');
            SolidIcons.Add("cp-contest-store", '\u0044');
            SolidIcons.Add("cp-double-coins", '\u0045');
            SolidIcons.Add("cp-double-tl-coin", '\u0046');
            SolidIcons.Add("cp-edit", '\u0047');
            SolidIcons.Add("cp-facebook", '\u0048');
            SolidIcons.Add("cp-find-opponent", '\u0049');
            SolidIcons.Add("cp-instagram", '\u004a');
            SolidIcons.Add("cp-likes", '\u004b');
            SolidIcons.Add("cp-logo", '\u004c');
            SolidIcons.Add("cp-menu", '\u004d');
            SolidIcons.Add("cp-mission-done", '\u004e');
            SolidIcons.Add("cp-missions", '\u004f');
            SolidIcons.Add("cp-options", '\u0050');
            SolidIcons.Add("cp-prize-1", '\u0051');
            SolidIcons.Add("cp-prize-2", '\u0052');
            SolidIcons.Add("cp-prize-3", '\u0053');
            SolidIcons.Add("cp-prize-4", '\u0054');
            SolidIcons.Add("cp-prize-5", '\u0055');
            SolidIcons.Add("cp-prize-6", '\u0056');
            SolidIcons.Add("cp-profile-chat", '\u0057');
            SolidIcons.Add("cp-profile-follow", '\u0058');
            SolidIcons.Add("cp-profile-play", '\u0059');
            SolidIcons.Add("cp-question-center", '\u005a');
            SolidIcons.Add("cp-revenge", '\u0061');
            SolidIcons.Add("cp-search", '\u0062');
            SolidIcons.Add("cp-search-cancel", '\u0063');
            SolidIcons.Add("cp-search-result", '\u0064');
            SolidIcons.Add("cp-send-message", '\u0065');
            SolidIcons.Add("cp-settings", '\u0066');
            SolidIcons.Add("cp-settings-account", '\u0067');
            SolidIcons.Add("cp-settings-blocked", '\u0068');
            SolidIcons.Add("cp-settings-edit-profile-cover-photo", '\u0069');
            SolidIcons.Add("cp-settings-edit-profile-email", '\u006a');
            SolidIcons.Add("cp-settings-edit-profile-name", '\u006b');
            SolidIcons.Add("cp-settings-edit-profile-profile-photo", '\u006c');
            SolidIcons.Add("cp-settings-edit-profile-username", '\u006d');
            SolidIcons.Add("cp-settings-language", '\u006e');
            SolidIcons.Add("cp-settings-log-out", '\u006f');
            SolidIcons.Add("cp-settings-private-profile", '\u0070');
            SolidIcons.Add("cp-share", '\u0071');
            SolidIcons.Add("cp-signle-coin", '\u0073');
            SolidIcons.Add("cp-sound", '\u0074');
            SolidIcons.Add("cp-store-coin-bag-1", '\u0075');
            SolidIcons.Add("cp-tab-chat", '\u0076');
            SolidIcons.Add("cp-tab-notification", '\u0077');
            SolidIcons.Add("cp-tick", '\u0078');
            SolidIcons.Add("cp-turkish-lira-bag", '\u0079');
            SolidIcons.Add("cp-turkish-lira-single-coin", '\u007a');
            SolidIcons.Add("cp-twitter", '\u0030');
            SolidIcons.Add("cp-user-profile", '\u0031');
            SolidIcons.Add("cp-verify", '\u0032');
            SolidIcons.Add("cp-vs", '\u0033');
            SolidIcons.Add("cp-arrow-left", '\u0034');
            SolidIcons.Add("cp-arrow-right", '\u0035');
            SolidIcons.Add("cp-cancel-1", '\u0036');
            SolidIcons.Add("cp-cancel-2", '\u0037');
            SolidIcons.Add("cp-cancel-3", '\u0038');
            SolidIcons.Add("cp-categories", '\u0039');

            #endregion Solid
        }
    }
}
