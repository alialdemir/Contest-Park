namespace ContestPark.Mobile.Models.Duel
{
    public class SelectedSubCategoryModel : BaseModel
    {
        public short SubcategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public string SubCategoryPicturePath { get; set; }

        public string OpponentUserId { get; set; }
    }
}
