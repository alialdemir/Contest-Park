using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Base;
using ContestPark.Mobile.Models.ServiceModel;
using MvvmHelpers;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels.Base
{
    public abstract class ViewModelBase : ExtendedBindableObject, IInitialize, INavigatedAware
    {
        #region Private variables

        private readonly IPageDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IPopupNavigation _popupNavigation;

        #endregion Private variables

        #region Constructor

        public ViewModelBase(INavigationService navigationService = null,
                             IPageDialogService dialogService = null,
                             IPopupNavigation popupNavigation = null)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _popupNavigation = popupNavigation;
            IsBusy = false;
            Title = String.Empty;
            IsShowEmptyMessage = false;
        }

        #endregion Constructor

        #region Page settings

        private bool _isShowEmptyMessage;

        public bool IsInitialized { get; set; } = false;

        public bool IsShowEmptyMessage
        {
            get
            {
                return _isShowEmptyMessage;
            }
            set
            {
                _isShowEmptyMessage = value;
                RaisePropertyChanged(() => IsShowEmptyMessage);
            }
        }

        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                RaisePropertyChanged(() => IsRefreshing);
            }
        }

        #endregion Page settings

        #region Virtoal methods

        /// <summary>
        /// Sayfalarda ortak load işlemleri burada yapılmalı ve refleshs olunca da bu çağrılır
        /// </summary>
        public virtual void Initialize(INavigationParameters parameters = null)
        {
        }

        #endregion Virtoal methods

        #region Dialogs

        protected Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            if (_dialogService == null)
                return Task.FromResult(string.Empty);

            return _dialogService?.DisplayActionSheetAsync(title, cancelButton, destroyButton, otherButtons);
        }

        protected Task DisplayActionSheetAsync(string title, params IActionSheetButton[] buttons)
        {
            if (_dialogService == null)
                return Task.CompletedTask;

            return _dialogService?.DisplayActionSheetAsync(title, buttons);
        }

        protected Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton)
        {
            if (_dialogService == null)
                return Task.FromResult(false);

            return _dialogService?.DisplayAlertAsync(title, message, acceptButton, cancelButton);
        }

        protected Task DisplayAlertAsync(string title, string message, string cancelButton)
        {
            if (_dialogService == null)
                return Task.CompletedTask;

            return _dialogService?.DisplayAlertAsync(title, message, cancelButton);
        }

        #endregion Dialogs

        #region Navigations

        public virtual Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            if (_navigationService == null)
                return Task.CompletedTask;

            return _navigationService.GoBackAsync(parameters, useModalNavigation);
        }

        public Task NavigateToPopupAsync<TViewModel>(INavigationParameters parameters = null)
        {
            string name = typeof(TViewModel).Name;
            if (string.IsNullOrEmpty(name))
                return Task.CompletedTask;

            return NavigateToAsync(name, parameters, useModalNavigation: true);
        }

        public Task NavigateToAsync<TView>(INavigationParameters parameters = null) where TView : ContentPage
        {
            string name = typeof(TView).Name;
            if (string.IsNullOrEmpty(name) || _navigationService == null)
                return Task.CompletedTask;

            return NavigateToAsync(name, parameters, useModalNavigation: false);
        }

        public Task NavigateToInitialized<TView>()
        {
            string name = typeof(TView).Name;
            if (string.IsNullOrEmpty(name))
                return Task.CompletedTask;

            return NavigateToAsync($"app:///{name}?appModuleRefresh=OnInitialized", null, false);
        }

        internal Task NavigateToAsync(string name, INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            if (string.IsNullOrEmpty(name) || _navigationService == null)
                return Task.CompletedTask;

            return _navigationService.NavigateAsync(name, parameters, useModalNavigation);
        }

        public string CurrentPopupName()
        {
            if (_popupNavigation == null || !_popupNavigation.PopupStack.Any())
                return string.Empty;

            PopupPage popupPage = _popupNavigation.PopupStack.FirstOrDefault();
            if (popupPage == null)
                return string.Empty;

            return popupPage.GetType().Name;
        }

        public Task RemoveFirstPopupAsync<TView>()
        {
            if (_popupNavigation == null || !_popupNavigation.PopupStack.Any(page => page.GetType() == typeof(TView)))
                return Task.CompletedTask;

            PopupPage popupPage = _popupNavigation.PopupStack.FirstOrDefault(page => page.GetType() == typeof(TView));
            if (popupPage == null)
                return Task.CompletedTask;

            return _popupNavigation?.RemovePageAsync(popupPage);
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        #endregion Navigations

        #region Commands

        public ICommand GotoBackCommand
        {
            get
            {
                return new Command<bool?>((useModalNavigation) => GoBackAsync(useModalNavigation: useModalNavigation));
            }
        }

        /// <summary>
        /// Veri yükle command
        /// </summary>
        public ICommand InitializeCommand => new Command<INavigationParameters>(Initialize);

        #endregion Commands
    }

    public abstract class ViewModelBase<TModel> : ViewModelBase where TModel : IModelBase

    {
        #region Constructor

        public ViewModelBase(INavigationService navigationService = null,
                             IPageDialogService dialogService = null,
                             IPopupNavigation popupNavigation = null,
                             bool isActiveSkeletonLoading = false) : base(navigationService, dialogService, popupNavigation)
        {
            if (isActiveSkeletonLoading)
            {
                Items.Add(default(TModel));
                Items.Add(default(TModel));
                Items.Add(default(TModel));
                Items.Add(default(TModel));
                Items.Add(default(TModel));
                Items.Add(default(TModel));
                Items.Add(default(TModel));
            }
        }

        #endregion Constructor

        #region Virtual methods

        protected ServiceModel<TModel> _serviceModel;
        private ObservableRangeCollection<TModel> _items;

        /// <summary>
        /// Listview içerisine bind edilecek liste
        /// </summary>
        public ObservableRangeCollection<TModel> Items
        {
            get { return _items ?? (_items = new ObservableRangeCollection<TModel>()); }
        }

        /// <summary>
        /// Sayfalarda ortak load işlemleri burada yapılmalı ve refleshs olunca da bu çağrılır
        /// </summary>
        public ServiceModel<TModel> ServiceModel
        {
            get
            {
                return _serviceModel ?? (_serviceModel = new ServiceModel<TModel>());
            }
            protected set
            {
                if (value != null)
                {
                    _serviceModel = value;

                    if (ServiceModel.PageNumber == 1)
                        Items.Clear();// Skeleton dataları silindi

                    if (ServiceModel != null && ServiceModel.Items != null && ServiceModel.Items.Any())
                    {
                        IsShowEmptyMessage = false;
                        Items.AddRange(ServiceModel.Items);
                    }
                    else IsShowEmptyMessage = true;

                    if (ServiceModel != null && ServiceModel.HasNextPage)
                        ServiceModel.PageNumber++;

                    ServiceModel.Items = null;

                    IsRefreshing = false;
                }
            }
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
        }

        /// <summary>
        /// Sayfa reflesh olunca yapılması gereken ortak işlemler
        /// </summary>
        protected virtual void Reflesh()
        {
            IsRefreshing = true;

            IsShowEmptyMessage = false;
            Items?.Clear();

            if (_serviceModel != null)
            {
                _serviceModel.Count = 0;
                _serviceModel.Items = null;
                _serviceModel.PageNumber = 1;
            }

            InitializeCommand.Execute(new NavigationParameters());
        }

        #endregion Virtual methods

        #region Commands

        /// <summary>
        /// Sayfalama için scrollbar aşayağıya gelince tekiklenen command
        /// </summary>
        public Command<BaseModel> InfiniteScroll
        {
            get
            {
                return new Command<BaseModel>((currentItem) =>
                {
                    if (!ServiceModel.HasNextPage || !(currentItem is BaseModel))
                        return;

                    if (Items.Count > 4 && Items[Items.Count - 2].Equals(currentItem))
                    {
                        InitializeCommand.Execute(new NavigationParameters());
                    }
                    else if (Items.LastOrDefault().Equals(currentItem))
                        InitializeCommand.Execute(new NavigationParameters());
                });
            }
        }

        /// <summary>
        /// Reflesh command
        /// </summary>
        public ICommand RefreshCommand => new Command(() => Reflesh());

        #endregion Commands
    }
}
