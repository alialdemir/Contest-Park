using ContestPark.Mobile.Enums;
using Newtonsoft.Json;
using System;

namespace ContestPark.Mobile.Models.Post
{
    public partial class PostModel : BaseModel
    {
        private int _commentCount;
        private bool _isLike;
        private int _likeCount;

        public int CommentCount
        {
            get { return _commentCount; }
            set
            {
                if (value >= 0)
                {
                    _commentCount = value;
                    RaisePropertyChanged(() => CommentCount);
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
                RaisePropertyChanged(() => IsLike);
                RaisePropertyChanged(() => LikeSource);
            }
        }

        [JsonIgnore]
        public string LikeSource
        {
            get
            {
                return IsLike ? "#ffc107" : "#6C7B8A";
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
                    RaisePropertyChanged(() => LikeCount);
                }
            }
        }

        public string OwnerFullName { get; set; }
        public string OwnerProfilePicturePath { get; set; }
        public string OwnerUserName { get; set; }
        public int PostId { get; set; }

        public PostTypes PostType { get; set; }
    }
}
