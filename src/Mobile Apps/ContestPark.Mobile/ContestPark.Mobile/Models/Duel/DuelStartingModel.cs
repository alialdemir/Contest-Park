using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using System;

namespace ContestPark.Mobile.Models.Duel
{
    public class DuelStartingModel : BaseModel
    {
        private string _founderCoverPicturePath = DefaultImages.DefaultCoverPicture;
        private string _founderFullName;
        private string _founderProfilePicturePath = DefaultImages.DefaultProfilePicture;

        private string _opponentCoverPicturePath = DefaultImages.DefaultCoverPicture;

        private string _opponentFullName;

        private string _opponentProfilePicturePath = DefaultImages.DefaultProfilePicture;

        public int DuelId { get; set; }

        public string FounderCoverPicturePath
        {
            get { return _founderCoverPicturePath; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _founderCoverPicturePath = value;
                    RaisePropertyChanged(() => FounderCoverPicturePath);
                }
            }
        }

        public string FounderFullName
        {
            get { return _founderFullName; }
            set
            {
                _founderFullName = value;
                RaisePropertyChanged(() => FounderFullName);
            }
        }

        public string FounderProfilePicturePath
        {
            get { return _founderProfilePicturePath; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _founderProfilePicturePath = value;
                    RaisePropertyChanged(() => FounderProfilePicturePath);
                }
            }
        }

        public string FounderUserId { get; set; }

        public string OpponentCoverPicturePath
        {
            get { return _opponentCoverPicturePath; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _opponentCoverPicturePath = value;
                    RaisePropertyChanged(() => OpponentCoverPicturePath);
                }
            }
        }

        public string OpponentFullName
        {
            get { return _opponentFullName; }
            set
            {
                _opponentFullName = value;
                RaisePropertyChanged(() => OpponentFullName);
            }
        }

        public string OpponentProfilePicturePath
        {
            get { return _opponentProfilePicturePath; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _opponentProfilePicturePath = value;
                    RaisePropertyChanged(() => OpponentProfilePicturePath);
                }
            }
        }

        public string OpponentUserId { get; set; }

        private byte _founderLevel = 1;

        public byte FounderLevel
        {
            get { return _founderLevel; }
            set
            {
                _founderLevel = value;
                RaisePropertyChanged(() => FounderLevel);
            }
        }

        private byte _opponentLevel = 1;

        public byte OpponentLevel
        {
            get { return _opponentLevel; }
            set
            {
                _opponentLevel = value;
                RaisePropertyChanged(() => OpponentLevel);
            }
        }

        private string _founderCountry = ContestParkResources.Turkey;

        public string FounderCountry
        {
            get { return _founderCountry; }
            set
            {
                _founderCountry = value;
                RaisePropertyChanged(() => FounderCountry);
            }
        }

        private string _opponentCountry = ContestParkResources.Turkey;

        public string OpponentCountry
        {
            get { return _opponentCountry; }
            set
            {
                _opponentCountry = value;
                RaisePropertyChanged(() => OpponentCountry);
            }
        }

        public string FounderConnectionId { get; }
        public string OpponentConnectionId { get; }
    }
}
