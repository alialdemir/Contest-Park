using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using Newtonsoft.Json;
using Prism.Ioc;

namespace ContestPark.Mobile.Models.Duel.DuelResult
{
    public class DuelResultModel : ExtendedBindableObject
    {
        private ISettingsService _settingsService;

        public Enums.Coins Coins
        {
            get
            {
                if (OpponentScore > FounderScore)
                {
                    return SettingsService.CurrentUser.UserId == OpponentUserId ? Enums.Coins.Positive : Enums.Coins.Negative;
                }
                else if (OpponentScore < FounderScore)
                {
                    return SettingsService.CurrentUser.UserId == FounderUserId ? Enums.Coins.Positive : Enums.Coins.Negative;
                }
                else
                {
                    return Enums.Coins.None;
                }
            }
        }

        public byte FinishBonus { get; set; }

        public byte MatchScore
        {
            get
            {
                if (SettingsService.CurrentUser.UserId == FounderUserId)
                {
                    return FounderScore;
                }
                else if (SettingsService.CurrentUser.UserId == OpponentUserId)
                {
                    return OpponentScore;
                }

                return 0;// eğer kurucu veya rakip dışında biri bakıyorsa 0 gözüksün
            }
        }

        public byte FounderScore { get; set; }

        public byte OpponentScore { get; set; }

        public byte? VictoryBonus { get; set; }

        [JsonIgnore]
        public int TotalXp
        {
            get { return FinishBonus + (VictoryBonus ?? 0) + MatchScore; }
        }

        [JsonIgnore]
        public string FounderColor
        {
            get
            {
                if (FounderScore > OpponentScore)
                {
                    return Green;
                }
                else if (FounderScore < OpponentScore)
                {
                    return Red;
                }
                else
                {
                    return Yellow;
                }
            }
        }

        public string FounderFullName { get; set; }
        public short FounderLevel { get; set; }
        public string FounderProfilePicturePath { get; set; }
        public string FounderUserId { get; set; }
        public string FounderUserName { get; set; }
        public decimal Gold { get; set; }

        public bool IsFounder { get; set; }

        [JsonIgnore]
        public bool IsShowFireworks
        {
            get
            {
                if (OpponentScore > FounderScore)
                {
                    return SettingsService.CurrentUser.UserId == OpponentUserId;
                }
                else if (OpponentScore < FounderScore)
                {
                    return SettingsService.CurrentUser.UserId == FounderUserId;
                }
                else
                {
                    return false;
                }
            }
        }

        [JsonIgnore]
        public string OpponentColor
        {
            get
            {
                if (OpponentScore > FounderScore)
                {
                    return Green;
                }
                else if (OpponentScore < FounderScore)
                {
                    return Red;
                }
                else
                {
                    return Yellow;
                }
            }
        }

        public string OpponentFullName { get; set; }
        public short OpponentLevel { get; set; }
        public string OpponentProfilePicturePath { get; set; }

        public string OpponentUserId { get; set; }

        public string OpponentUserName { get; set; }

        public short SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public string SubCategoryPicturePath { get; set; }

        public BalanceTypes BalanceType { get; set; }

        public string WinnerOrLoseText
        {
            get
            {
                if (OpponentScore > FounderScore)
                {
                    return SettingsService.CurrentUser.UserId == OpponentUserId ? ContestParkResources.YouWin : ContestParkResources.YouLose;
                }
                else if (OpponentScore < FounderScore)
                {
                    return SettingsService.CurrentUser.UserId == FounderUserId ? ContestParkResources.YouWin : ContestParkResources.YouLose;
                }
                else
                {
                    return ContestParkResources.Tie;
                }
            }
        }

        [JsonIgnore]
        public string WinnerOrLoseTextColor
        {
            get
            {
                if (OpponentScore > FounderScore)
                {
                    return SettingsService.CurrentUser.UserId == OpponentUserId ? Green : Red;
                }
                else if (OpponentScore < FounderScore)
                {
                    return SettingsService.CurrentUser.UserId == FounderUserId ? Green : Red;
                }
                else
                {
                    return Yellow;
                }
            }
        }

        [JsonIgnore]
        public string Red
        {
            get
            {
                return "#FE5353";
            }
        }

        [JsonIgnore]
        public string Yellow
        {
            get
            {
                return "#FFC107";
            }
        }

        [JsonIgnore]
        public string Green
        {
            get
            {
                return "#02CD72";
            }
        }

        private ISettingsService SettingsService
        {
            get
            {
                if (_settingsService == null)
                {
                    _settingsService = ContestParkApp.Current.Container.Resolve<ISettingsService>();
                }

                return _settingsService;
            }
        }
    }
}
