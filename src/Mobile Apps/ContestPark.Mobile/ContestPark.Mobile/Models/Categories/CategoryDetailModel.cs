using System.ComponentModel;

namespace ContestPark.Mobile.Models.Categories.CategoryDetail
{
    public class CategoryDetailModel : INotifyPropertyChanged
    {
        private bool _isSubCategoryFollowUpStatus;
        private byte _level = 1;
        private string _subCategoryPicturePath;

        private int categoryFollowersCount;

        private string description;

        private string subCategoryName;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Kategoriyi takip eden kullanıcı sayısı
        /// </summary>
        public int CategoryFollowersCount
        {
            get { return categoryFollowersCount; }
            set
            {
                categoryFollowersCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CategoryFollowersCount)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSubCategoryFollowUpStatus)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Level)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubCategoryName)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubCategoryPicturePath)));
            }
        }
    }
}