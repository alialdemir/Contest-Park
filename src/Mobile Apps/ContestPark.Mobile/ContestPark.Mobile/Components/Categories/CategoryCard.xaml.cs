using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    
    public partial class CategoryCard : ContentView
    {
        #region Methods

        public CategoryCard()
        {
            InitializeComponent();
        }

        #endregion Methods

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

        public static readonly BindableProperty GoToCategorySearchPageCommandProperty = BindableProperty.Create(propertyName: nameof(GoToCategorySearchPageCommand),
                                                                                                                        returnType: typeof(ICommand),
                                                                                                                        declaringType: typeof(CategoryCard),
                                                                                                                        defaultValue: null);

        /// <summary>
        /// Alt kategori display alert command
        /// </summary>
        public ICommand GoToCategorySearchPageCommand
        {
            get { return (ICommand)GetValue(GoToCategorySearchPageCommandProperty); }
            set { SetValue(GoToCategorySearchPageCommandProperty, value); }
        }

        public static readonly BindableProperty SubCategoriesDisplayActionSheetCommandProperty = BindableProperty.Create(propertyName: nameof(SubCategoriesDisplayActionSheetCommand),
                                                                                                                        returnType: typeof(ICommand),
                                                                                                                        declaringType: typeof(CategoryCard),
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
                                                                                                                declaringType: typeof(CategoryCard),
                                                                                                                defaultValue: null);

        public ICommand PushCategoryDetailViewCommand
        {
            get { return (ICommand)GetValue(PushCategoryDetailViewCommandProperty); }
            set { SetValue(PushCategoryDetailViewCommandProperty, value); }
        }

        #endregion Properties
    }
}
