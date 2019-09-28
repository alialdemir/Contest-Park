﻿using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Quiz;
using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionPopupView : PopupPage
    {
        #region Constructor

        public QuestionPopupView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public DuelCreated DuelCreated { get; set; }

        public DuelStartingModel DuelStarting { get; set; }

        public string SubcategoryName { get; set; }

        public string SubCategoryPicturePath { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = ((QuestionPopupViewModel)BindingContext);
            if (viewModel == null || viewModel.IsInitialized)
                return;

            viewModel.IsInitialized = true;

            viewModel.DuelCreated = DuelCreated;
            viewModel.DuelScreen = DuelStarting;
            viewModel.SubcategoryName = SubcategoryName;
            viewModel.SubCategoryPicturePath = SubCategoryPicturePath;
            //viewModel.AnimateStylishCommand = new Command(Stylishs.AnimateStylish);

            viewModel.InitializeCommand.Execute(null);
        }

        protected override bool OnBackButtonPressed()
        {
            ((QuestionPopupViewModel)BindingContext).DuelCloseCommand.Execute(true);
            return true;
        }

        #endregion Methods
    }
}
