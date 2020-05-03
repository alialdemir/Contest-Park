using ContestPark.Mobile.Models.Categories;
using FFImageLoading.Transformations;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    
    public partial class SubCategoryCard : ContentView
    {
        #region Constructor

        public SubCategoryCard()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty IsBlurredTransformationProperty = BindableProperty.Create(propertyName: nameof(IsBlurredTransformation),
                                                                                                                  returnType: typeof(bool),
                                                                                                                  declaringType: typeof(SubCategoryCard),
                                                                                                                  defaultValue: false);

        /// <summary>
        /// Alt kategori display alert command
        /// </summary>
        public bool IsBlurredTransformation
        {
            get { return (bool)GetValue(IsBlurredTransformationProperty); }
            set { SetValue(IsBlurredTransformationProperty, value); }
        }

        public static readonly BindableProperty SubCategoriesDisplayActionSheetCommandProperty = BindableProperty.Create(propertyName: nameof(SubCategoriesDisplayActionSheetCommand),
                                                                                                                        returnType: typeof(ICommand),
                                                                                                                        declaringType: typeof(SubCategoryCard),
                                                                                                                        defaultValue: null);

        /// <summary>
        /// Alt kategori display alert command
        /// </summary>
        public ICommand SubCategoriesDisplayActionSheetCommand
        {
            get { return (ICommand)GetValue(SubCategoriesDisplayActionSheetCommandProperty); }
            set { SetValue(SubCategoriesDisplayActionSheetCommandProperty, value); }
        }

        public static readonly BindableProperty PushCategoryDetailViewCommandProperty = BindableProperty.Create(propertyName: nameof(PushCategoryDetailViewCommand),
                                                                                                                returnType: typeof(ICommand),
                                                                                                                declaringType: typeof(SubCategoryCard),
                                                                                                                defaultValue: null);

        public ICommand PushCategoryDetailViewCommand
        {
            get { return (ICommand)GetValue(PushCategoryDetailViewCommandProperty); }
            set { SetValue(PushCategoryDetailViewCommandProperty, value); }
        }

        #endregion Properties

        #region Methods

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            SubCategoryModel subCategory = (SubCategoryModel)BindingContext;
            if (subCategory == null || !IsBlurredTransformation)
                return;

            subCategory.PropertyChanged += SubCategory_PropertyChanged;

            SubCategory_PropertyChanged(subCategory, null);
        }

        /// <summary>
        /// Alt kategori resmindeki blur kaldır göster
        /// </summary>
        private void SubCategory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SubCategoryModel subCategory = (SubCategoryModel)sender;
            if (subCategory == null || (e != null && e.PropertyName != nameof(subCategory.IsSubCategoryOpen)))
                return;

            bool isAnyBlurredTransformation = imgSubCategory.Transformations.Any(x => x.GetType() == typeof(BlurredTransformation));
            if (subCategory.IsSubCategoryOpen && isAnyBlurredTransformation)
            {
                imgSubCategory.Transformations.RemoveAll(x => x.GetType() == typeof(BlurredTransformation));
                imgSubCategory.ReloadImage();
            }
            else if (!isAnyBlurredTransformation)
            {
                imgSubCategory.Transformations.Add(new BlurredTransformation(40));
                imgSubCategory.ReloadImage();
            }
        }

        #endregion Methods
    }
}
