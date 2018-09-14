using Autofac;
using ContestPark.Mobile.Components.Categories;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Services.Game;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class SubCategoryView : ContentView
    {
        private bool IsBusy = false;

        #region Methods

        private void CategoryCreator()
        {
            if (!(BindingContext is CategoryModel categoryModel) || categoryModel.SubCategories == null) return;

            StackLayout stackmain = new StackLayout() { Spacing = 0, Orientation = StackOrientation.Vertical, BackgroundColor = Color.FromHex("#fff") };
            ScrollView scrollSubCategory = new ScrollView() { Orientation = ScrollOrientation.Horizontal };
            StackLayout stackSubCategory = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 0, Margin = new Thickness(0, 10, 0, 0) };

            foreach (var subCategory in categoryModel.SubCategories)
            {
                #region Grid info

                CustomGrid subCategoryGrid = new CustomGrid() { Padding = new Thickness(10, 0, 0, 0), ColumnSpacing = 0.1 };
                // Rows
                subCategoryGrid.RowDefinitions.Add(new RowDefinition() { Height = 80 });
                subCategoryGrid.RowDefinitions.Add(new RowDefinition() { Height = 20 });

                // Columns
                subCategoryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 40 });
                subCategoryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 40 });

                #region Sub category click

                subCategoryGrid.SingleTap = new Command(async () =>
                {
                    if (IsBusy) return;
                    IsBusy = true;

                    IGameService duelService = RegisterTypesConfig.Container.Resolve<IGameService>();
                    await duelService?.PushCategoryDetailViewAsync(subCategory);

                    IsBusy = false;
                });

                #endregion Sub category click

                #region Sub category long press

                subCategoryGrid.LongPressed = new Command(() =>
                {
                    if (IsBusy) return;
                    IsBusy = true;
                    LongPressed?.Execute(subCategory);
                    IsBusy = false;
                });

                #endregion Sub category long press

                #endregion Grid info

                #region Sub category Image

                var imgSubCategory = new FFImageLoading.Forms.CachedImage()
                {
                    WidthRequest = 70,
                    HeightRequest = 70,
                    Source = subCategory.PicturePath,
                    DownsampleToViewSize = true,
                    Transformations = new List<ITransformation>() { new CircleTransformation() },
                    Aspect = Aspect.AspectFill,
                    CacheDuration = TimeSpan.FromDays(365)
                };
                subCategoryGrid.Children.Add(imgSubCategory, 0, 0);
                Grid.SetColumnSpan(imgSubCategory, 2);

                #region Kategori kilitli ise çıkan kategori fiyatı ve coins resmi

                if (!subCategory.IsCategoryOpen)
                {
                    AbsoluteLayout absoluteLayout = new AbsoluteLayout();

                    Frame frame = new Frame
                    {
                        CornerRadius = 30,
                        HeightRequest = 80,
                        WidthRequest = 80,
                        BackgroundColor = (Color)ContestParkApp.Current.Resources["Primary"],
#pragma warning disable CS0618 // Type or member is obsolete
                        OutlineColor = Color.FromHex("#ebedf0"),
#pragma warning restore CS0618 // Type or member is obsolete
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.End,
                        Margin = 0,
                        Padding = 0,
                        Content = new Label
                        {
                            Text = subCategory.DisplayPrice,
                            TextColor = Color.Black,
                            Margin = 0,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center
                        }
                    };

                    AbsoluteLayout.SetLayoutBounds(frame, new Rectangle(.9, 45, 40, 40));
                    AbsoluteLayout.SetLayoutFlags(frame, AbsoluteLayoutFlags.XProportional);

                    absoluteLayout.Children.Add(frame);

                    subCategoryGrid.Children.Add(absoluteLayout, 1, 0);
                }

                #endregion Kategori kilitli ise çıkan kategori fiyatı ve coins resmi

                #endregion Sub category Image

                #region Sub category name label

                Label lblSubCategoryName = new Label()
                {
                    //FontSize = 15,
                    TextColor = Color.FromHex("#333"),
                    Text = subCategory.SubCategoryName,
                    FontAttributes = FontAttributes.Bold,
                    LineBreakMode = LineBreakMode.CharacterWrap,
                    HorizontalTextAlignment = TextAlignment.Center,
                    AutomationId = "lblSubCategoryName"
                };
                subCategoryGrid.Children.Add(lblSubCategoryName, 0, 1);
                Grid.SetColumnSpan(lblSubCategoryName, 2);

                #endregion Sub category name label

                stackSubCategory.Children.Add(subCategoryGrid);
                stackmain.Children.Add(scrollSubCategory);
            }
            scrollSubCategory.Content = stackSubCategory;

            #region Zigzag

            stackmain.Children.Add(new Image()
            {
                Source = "zigzag.png",
                Aspect = Aspect.Fill,
            });

            #endregion Zigzag

            Content = stackmain;
        }

        #endregion Methods

        #region Property

        public static readonly BindableProperty LongPressedProperty = BindableProperty.Create(propertyName: nameof(LongPressed),
                                                                                                returnType: typeof(ICommand),
                                                                                                declaringType: typeof(ICommand),
                                                                                                defaultValue: null);

        public ICommand LongPressed
        {
            get { return (ICommand)GetValue(LongPressedProperty); }
            set { SetValue(LongPressedProperty, value); }
        }

        #endregion Property

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            CategoryCreator();
        }

        #endregion Override
    }
}