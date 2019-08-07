using ContestPark.Mobile.Models.Categories;
using System.Windows.Input;
using Xamarin.Forms;

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

            StackLayout stackLayout = new StackLayout();

            stackLayout.Children.Add(new CategoryHeader
            {
                CategoryName = categoryModel.CategoryName,
                SeeAllCommandParameter = categoryModel.CategoryId,
                SeeAllPressed = SeeAllPressed,
            });

            stackLayout.Children.Add(new SubCategoryHorizontalScrollView());

            stackLayout.Children.Add(new Line());

            Content = stackLayout;
        }

        #endregion Override
    }
}
