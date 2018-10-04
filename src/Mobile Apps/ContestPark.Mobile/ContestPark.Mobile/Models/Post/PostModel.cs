using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
using System;

namespace ContestPark.Mobile.Models.Post
{
    public class PostModel : BaseModel
    {
        public PostTypes PostType { get; set; }
        private string _postsDescription;

        public string PostsDescription
        {
            get { return _postsDescription?.Replace("{yarisma}", SubCategoryName).Replace("{kullaniciadi}", CompetitorFullName); }
            set { _postsDescription = value; }
        }

        public string AlternativePicturePath { get; set; }
        public string AlternativeId { get; set; }
        public DateTime Date { get; set; }
        public int LikeCount { get; set; }
        public int PostId { get; set; }
        private bool _isLike;

        public bool IsLike
        {
            get { return _isLike; }
            set
            {
                _isLike = value;
                OnPropertyChanged(nameof(IsLike));
            }
        }

        public int CommentCount { get; set; }
        public short SubCategoryId { get; set; }
        public string FounderFullName { get; set; }
        public string FounderUserName { get; set; }
        private string founderProfilePicturePath = DefaultImages.DefaultProfilePicture;

        public string FounderProfilePicturePath
        {
            get
            {
                return founderProfilePicturePath;
            }
            set
            {
                if (!String.IsNullOrEmpty(value)) founderProfilePicturePath = value;
            }
        }

        public int FounderTrueAnswerCount { get; set; }
        private string competitorFullName;

        public string CompetitorFullName
        {
            get { return competitorFullName; }
            set
            {
                if (PostType.HasFlag(PostTypes.CoverPictureChanged) || PostType.HasFlag(PostTypes.ProfilePictureChanged))
                    return;

                competitorFullName = value;
            }
        }

        private string competitorUserName;

        public string CompetitorUserName
        {
            get { return competitorUserName; }
            set
            {
                if (PostType.HasFlag(PostTypes.CoverPictureChanged) || PostType.HasFlag(PostTypes.ProfilePictureChanged))
                    return;

                competitorUserName = value;
            }
        }

        private string competitorProfilePicturePath = DefaultImages.DefaultProfilePicture;

        public string CompetitorProfilePicturePath
        {
            get
            {
                if (PostType.HasFlag(PostTypes.CoverPictureChanged) || PostType.HasFlag(PostTypes.ProfilePictureChanged))
                    return String.Empty;

                return competitorProfilePicturePath;
            }
            set
            {
                if (!String.IsNullOrEmpty(value)) competitorProfilePicturePath = value;
            }
        }

        public int CompetitorTrueAnswerCount { get; set; }
        public string SubCategoryName { get; set; }

        public string FounderColor
        {
            get
            {
                if (FounderTrueAnswerCount == CompetitorTrueAnswerCount) return "#FFC200";//sarı
                else if (FounderTrueAnswerCount > CompetitorTrueAnswerCount) return "#017d46";//yeşil
                return "#993232";//kırmız
            }
        }

        public string CompetitorColor
        {
            get
            {
                if (FounderTrueAnswerCount == CompetitorTrueAnswerCount) return "#FFC200";//sarı
                else if (CompetitorTrueAnswerCount > FounderTrueAnswerCount) return "#017d46";//yeşil
                return "#993232";//kırmız
            }
        }

        public string FounderWinnerOrLose
        {
            get
            {
                if (PostType.HasFlag(PostTypes.CoverPictureChanged) || PostType.HasFlag(PostTypes.ProfilePictureChanged))
                    return String.Empty;

                if (FounderTrueAnswerCount == CompetitorTrueAnswerCount) return "Tie";//sarı
                else if (FounderTrueAnswerCount > CompetitorTrueAnswerCount) return "Winning";//yeşil
                return "Lose";//kırmız
            }
        }

        public string CompetitorWinnerOrLose
        {
            get
            {
                if (PostType.HasFlag(PostTypes.CoverPictureChanged) || PostType.HasFlag(PostTypes.ProfilePictureChanged))
                    return String.Empty;

                if (FounderTrueAnswerCount == CompetitorTrueAnswerCount) return "Tie";//sarı
                else if (CompetitorTrueAnswerCount > FounderTrueAnswerCount) return "Winning";//yeşil
                return "Lose";//kırmız
            }
        }
    }
}