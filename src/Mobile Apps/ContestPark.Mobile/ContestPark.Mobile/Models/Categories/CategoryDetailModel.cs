namespace ContestPark.Mobile.Models.Categories.CategoryDetail
{
    public class CategoryDetailModel : BaseModel
    {
        private bool _isSubCategoryFollowUpStatus;
        private byte _level = 1;
        private string _picturePath;

        private string description;

        private string subCategoryName;

        /// <summary>
        /// Kategori açıklaması
        /// </summary>
        public string Description
        {
            get { return description; }
            set
            {
                description = value;

                RaisePropertyChanged(() => Description);
            }
        }

        private string _İCON;

        public string Icon
        {
            get { return _İCON; }
            set
            {
                _İCON = value;
                RaisePropertyChanged(() => Icon);
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

        public byte Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;

                RaisePropertyChanged(() => Level);
            }
        }

        public short SubCategoryId { get; set; }

        /// <summary>
        /// Kategori açıklaması
        /// </summary>
        public string SubCategoryName
        {
            get { return subCategoryName; }
            set
            {
                subCategoryName = value;

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
