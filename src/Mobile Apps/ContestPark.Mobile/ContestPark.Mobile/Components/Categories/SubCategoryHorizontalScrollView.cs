﻿using ContestPark.Mobile.Models.Categories;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class SubCategoryHorizontalScrollView : ScrollView
    {
        #region Constructor

        public SubCategoryHorizontalScrollView()
        {
            Orientation = ScrollOrientation.Horizontal;
            HeightRequest = 120;
        }

        #endregion Constructor

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (!(BindingContext is CategoryModel categoryModel) ||
                categoryModel.SubCategories == null ||
                categoryModel.SubCategories.Count == 0)
            {
                return;
            }

            StackLayout stackLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 20 };

            categoryModel.SubCategories.ForEach(subCategory =>
            {
                stackLayout.Children.Add(new SubCategoryCard
                {
                    SubCategoryId = subCategory.SubCategoryId,
                    SubCategoryImageSource = subCategory.PicturePath,
                    SubCategoryName = subCategory.SubCategoryName,
                    DisplayPrice = subCategory.DisplayPrice,
                    IsCategoryOpen = subCategory.IsCategoryOpen
                });
            });

            Content = stackLayout;
        }

        #endregion Override
    }
}
