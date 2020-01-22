using ContestPark.Mobile.Models.Categories;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace ContestPark.Mobile.Components
{
    public class CategoryCard : ContentView
    {
        #region Properties

        public static readonly BindableProperty SeeAllPressedProperty = BindableProperty.Create(propertyName: nameof(SeeAllPressed),
                                                                                          returnType: typeof(ICommand),
                                                                                          declaringType: typeof(ICommand),
                                                                                          defaultValue: null);

        public ICommand SeeAllPressed
        {
            get { return (ICommand)GetValue(SeeAllPressedProperty); }
            set { SetValue(SeeAllPressedProperty, value); }
        }

        public static readonly BindableProperty SeeAllCommandParameterProperty = BindableProperty.Create(propertyName: nameof(SeeAllCommandParameter),
                                                                                                returnType: typeof(object),
                                                                                                declaringType: typeof(object),
                                                                                                defaultValue: null);

        public object SeeAllCommandParameter
        {
            get { return (object)GetValue(SeeAllCommandParameterProperty); }
            set { SetValue(SeeAllCommandParameterProperty, value); }
        }

        #endregion Properties

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            //Spacing = 0;

            if (!(BindingContext is CategoryModel categoryModel) ||
              categoryModel.SubCategories == null ||
              categoryModel.SubCategories.Count == 0)
            {
                return;
            }

            StackLayout stackLayout = new StackLayout() { Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };

            stackLayout.Children.Add(new CategoryHeader
            {
                CategoryName = categoryModel.CategoryName,
                SeeAllCommandParameter = categoryModel.CategoryId,
                SeeAllPressed = SeeAllPressed,
            });

            stackLayout.Children.Add(new SubCategoryHorizontalScrollView());

            PancakeView frame = new PancakeView
            {
                HasShadow = true,
                Elevation = 8,
                CornerRadius = 8,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IsClippedToBounds = true,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("#FFFFFF"),
                Content = stackLayout
            };

            Margin = new Thickness(0, 0, 0, 8);

            frame.Padding = new Thickness(8);

            Content = frame;
        }

        #endregion Override
    }
}
