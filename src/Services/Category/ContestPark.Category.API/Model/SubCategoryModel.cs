namespace ContestPark.Category.API.Model
{
    public class SubCategoryModel
    {
        public short SubCategoryId { get; set; }
        public string PicturePath { get; set; }
        public decimal Price { get; set; }
        private string _displayPrice = "0";

        public string DisplayPrice
        {
            get
            {
                return _displayPrice;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _displayPrice = value;
            }
        }

        public string SubCategoryName { get; set; }
    }
}
