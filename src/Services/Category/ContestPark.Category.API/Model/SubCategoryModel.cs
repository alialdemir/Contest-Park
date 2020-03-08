namespace ContestPark.Category.API.Model
{
    public class SubCategoryModel
    {
        public short SubCategoryId { get; set; }
        public string PicturePath { get; set; }
        public decimal Price { get; set; }
        public string DisplayPrice { get; set; }

        public string SubCategoryName { get; set; }
    }
}
