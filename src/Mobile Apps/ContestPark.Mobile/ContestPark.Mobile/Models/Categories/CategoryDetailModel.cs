namespace ContestPark.Mobile.Models.Categories.CategoryDetail
{
    public class CategoryDetailModel : BaseModel
    {
        private bool _isSubCategoryFollowUpStatus;
        private string _picturePath;

        private string _description;

        private string _subCategoryName;

        /// <summary>
        /// Kategori açıklaması
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;

                RaisePropertyChanged(() => Description);
            }
        }

        public bool IsSubCategoryFollowUpStatus
        {
            get
            {
                return _isSubCategoryFollowUpStatus;
            }
            set
            {
                _isSubCategoryFollowUpStatus = value;

                RaisePropertyChanged(() => IsSubCategoryFollowUpStatus);
            }
        }

        public short Level { get; set; } = 1;

        public short SubCategoryId { get; set; }

        /// <summary>
        /// Kategori açıklaması
        /// </summary>
        public string SubCategoryName
        {
            get { return _subCategoryName; }
            set
            {
                _subCategoryName = value;

                RaisePropertyChanged(() => SubCategoryName);
            }
        }

        /// <summary>
        /// Kategori resmi
        /// </summary>
        public string PicturePath
        {
            get
            {
                return _picturePath;
            }
            set
            {
                _picturePath = value;

                RaisePropertyChanged(() => PicturePath);
            }
        }

        private long _followerCount;

        public long FollowerCount
        {
            get { return _followerCount; }
            set
            {
                _followerCount = value;

                RaisePropertyChanged(() => FollowerCount);
            }
        }
    }
}
