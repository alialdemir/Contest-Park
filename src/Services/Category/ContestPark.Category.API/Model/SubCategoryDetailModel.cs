namespace ContestPark.Category.API.Model
{
    public class SubCategoryDetailModel : SubCategoryDetailInfoModel
    {
        public bool IsSubCategoryFollowUpStatus { get; set; }

        public byte Level { get; set; }
    }
}