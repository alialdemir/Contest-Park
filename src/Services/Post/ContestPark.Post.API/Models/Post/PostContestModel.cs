using ContestPark.Core.Models;
using ContestPark.Post.API.Enums;

namespace ContestPark.Post.API.Models.Post
{
    public partial class PostModel
    {
        private string competitorProfilePicturePath = DefaultImages.DefaultProfilePicture;
        private string founderProfilePicturePath = DefaultImages.DefaultProfilePicture;

        public decimal Bet { get; set; }
        public string DuelId { get; set; }

        public string SubCategoryId { get; set; }

        public string CompetitorUserId { get; set; }
        public string FounderUserId { get; set; }
        public byte CompetitorTrueAnswerCount { get; set; }
        public byte FounderTrueAnswerCount { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryPicturePath { get; set; }
        public string CompetitorFullName { get; set; }

        public string CompetitorProfilePicturePath
        {
            get
            {
                if (!PostType.HasFlag(PostTypes.Contest))
                    return string.Empty;

                return competitorProfilePicturePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value)) competitorProfilePicturePath = value;
            }
        }

        public string CompetitorUserName { get; set; }

        public string FounderFullName { get; set; }

        public string FounderProfilePicturePath
        {
            get
            {
                return founderProfilePicturePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value)) founderProfilePicturePath = value;
            }
        }

        public string FounderUserName { get; set; }
    }
}
