using ContestPark.Mobile.Helpers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ContestPark.Mobile.Models.Duel
{
    public class DuelStartingModel : BaseModel, INotifyPropertyChanged
    {
        private string _founderProfilePicturePath = DefaultImages.DefaultProfilePicture;

        public string FounderProfilePicturePath
        {
            get { return _founderProfilePicturePath; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _founderProfilePicturePath = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _founderCoverPicturePath = DefaultImages.DefaultCoverPicture;

        public string FounderCoverPicturePath
        {
            get { return _founderCoverPicturePath; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _founderCoverPicturePath = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _founderFullName;

        public string FounderFullName
        {
            get { return _founderFullName; }
            set
            {
                _founderFullName = value;
                OnPropertyChanged();
            }
        }

        private string _opponentProfilePicturePath = DefaultImages.DefaultProfilePicture;

        public string OpponentProfilePicturePath
        {
            get { return _opponentProfilePicturePath; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _opponentProfilePicturePath = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _opponentCoverPicturePath = DefaultImages.DefaultCoverPicture;

        public string OpponentCoverPicturePath
        {
            get { return _opponentCoverPicturePath; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _opponentCoverPicturePath = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _opponentFullName;

        public string OpponentFullName
        {
            get { return _opponentFullName; }
            set
            {
                _opponentFullName = value;
                OnPropertyChanged();
            }
        }

        //  public Int16 SubCategoryId { get; set; }

        public int DuelId { get; set; }

        //public string FounderConnectionId { get; set; }

        //public string OpponentConnectionId { get; set; }

        public string FounderUserId { get; set; }

        public string OpponentUserId { get; set; }

        //public Languages FounderLanguage { get; set; }

        //public Languages OpponentLanguage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}