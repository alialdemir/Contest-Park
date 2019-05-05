namespace ContestPark.Mobile.Models.Categories.CategoryDetail
{
    public class CategoryDetailModel : BaseModel
    {
        private bool _isSubCategoryFollowUpStatus;
        private byte _level = 1;
        private string _subCategoryPicturePath;

        private int categoryFollowersCount;

        private string description;

        private string subCategoryName;

        /// <summary>
        /// Kategoriyi takip eden kullanıcı sayısı
        /// </summary>
        public int CategoryFollowersCount
        {
            get { return categoryFollowersCount; }
            set
            {
                categoryFollowersCount = value;

                RaisePropertyChanged(() => CategoryFollowersCount);
            }
        }

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
        public string SubCategoryPicturePath
        {
            get
            {
                return _subCategoryPicturePath;
            }
            set
            {
                _subCategoryPicturePath = value;

                RaisePropertyChanged(() => SubCategoryPicturePath);
            }
        }
    }
}