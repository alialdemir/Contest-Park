﻿using ContestPark.Mobile.Models.Categories;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components.Search
{
    public class SearchFactory : ContentView
    {
        #region Properties

        public static readonly BindableProperty GotoProfilePageCommandProperty = BindableProperty.Create(propertyName: nameof(GotoProfilePageCommand),
                                                                                                         returnType: typeof(ICommand),
                                                                                                         declaringType: typeof(SearchFactory),
                                                                                                         defaultValue: null);

        public static readonly BindableProperty PushCategoryDetailCommandProperty = BindableProperty.Create(propertyName: nameof(PushCategoryDetailCommand),
                                                                                                            returnType: typeof(ICommand),
                                                                                                            declaringType: typeof(SearchFactory),
                                                                                                            defaultValue: null);

        public static readonly BindableProperty FollowCommandProperty = BindableProperty.Create(propertyName: nameof(FollowCommand),
                                                                                                returnType: typeof(ICommand),
                                                                                                declaringType: typeof(SearchFactory),
                                                                                                defaultValue: null);

        public ICommand GotoProfilePageCommand
        {
            get { return (ICommand)GetValue(GotoProfilePageCommandProperty); }
            set { SetValue(GotoProfilePageCommandProperty, value); }
        }

        public ICommand PushCategoryDetailCommand
        {
            get { return (ICommand)GetValue(PushCategoryDetailCommandProperty); }
            set { SetValue(PushCategoryDetailCommandProperty, value); }
        }

        public ICommand FollowCommand
        {
            get { return (ICommand)GetValue(FollowCommandProperty); }
            set { SetValue(FollowCommandProperty, value); }
        }

        #endregion Properties

        #region Overrides

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            SearchModel searchModel = (SearchModel)BindingContext;
            if (searchModel == null)
                return;

            switch (searchModel.SearchType)
            {
                case Enums.SearchTypes.Player:
                    Content = new UserFollowListItem()
                    {
                        Margin = new Thickness(8, 0, 8, 0),
                        GotoProfilePageCommand = GotoProfilePageCommand,
                        RightButtonCommand = FollowCommand,
                    };
                    break;

                case Enums.SearchTypes.Category:
                    Content = new CategorySearchView()
                    {
                        PushCategoryDetailCommand = PushCategoryDetailCommand,
                        BindingContext = searchModel
                    };
                    break;
            }
        }

        #endregion Overrides
    }
}
