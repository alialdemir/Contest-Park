namespace ContestPark.Category.API.Model
{
    public class SubCategoryDetailInfoModel
    {
        public bool IsSubCategoryFollowUpStatus { get; set; }

        public byte Level { get; set; } = 1;
        public int FollowerCount { get; set; }

        public string Description { get; set; }

        public short SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public string PicturePath { get; set; }
        public bool IsSubCategoryOpen { get; set; }
    }
}
