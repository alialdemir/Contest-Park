using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Base;
using System;
using System.ComponentModel;

namespace ContestPark.Mobile.Models.Post
{
    public partial class PostModel : IModelBase, INotifyPropertyChanged
    {
        private int _commentCount;
        private bool _isLike;
        private int _likeCount;

        public event PropertyChangedEventHandler PropertyChanged;

        public int CommentCount
        {
            get { return _commentCount; }
            set
            {
                if (value >= 0)
                {
                    _commentCount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CommentCount)));
                }
            }
        }

        public DateTime Date { get; set; }

        public bool IsLike
        {
            get { return _isLike; }
            set
            {
                _isLike = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLike)));
            }
        }

        public int LikeCount
        {
            get { return _likeCount; }
            set
            {
                if (value >= 0)
                {
                    _likeCount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LikeCount)));
                }
            }
        }

        public string OwnerFullName { get; set; }
        public string OwnerProfilePicturePath { get; set; }
        public string OwnerUserName { get; set; }
        public string PostId { get; set; }

        public PostTypes PostType { get; set; }
    }
}