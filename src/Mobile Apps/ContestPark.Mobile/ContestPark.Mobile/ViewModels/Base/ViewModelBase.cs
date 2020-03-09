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
    public abstract class ViewModelBase : ExtendedBindableObject, INavigationAware
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
        /// <returns></returns>
        protected virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
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

        public virtual Task GoBackAsync(bool? useModalNavigation = false)
        {
            if (_navigationService == null)
                return Task.CompletedTask;

            return _navigationService?.GoBackAsync(useModalNavigation: useModalNavigation);
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            if (IsInitialized)
                return;

            Device.BeginInvokeOnMainThread(() =>// UI thread kitlememesi için mainthread üzerinden initialize ettik
            {
                InitializeCommand.Execute(null);
                IsInitialized = true;
            });
        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {
        }

        public Task PushModalAsync(string name, INavigationParameters parameters = null)
        {
            if (string.IsNullOrEmpty(name))
                return Task.CompletedTask;

            return PushNavigationPageAsync(name, parameters, useModalNavigation: true);
        }

        public Task PushNavigationPageAsync(string name, INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            if (string.IsNullOrEmpty(name) || _navigationService == null)
                return Task.CompletedTask;

            return _navigationService.NavigateAsync(name, parameters, useModalNavigation);
        }

        public Task PushPopupPageAsync(PopupPage page)
        {
            if (page == null || _popupNavigation == null)
                return Task.CompletedTask;
            // TODO: burada IPopupNavigation kullanmak yerine INavigationService ile popup açabiliyoruz ancak prism popup bug varmış o güncellendikten sonra IPopupNavigation iptal edilmeli: link: https://github.com/dansiegel/Prism.Plugin.Popups/pull/136
            return _popupNavigation.PushAsync(page);
        }

        public Task RemoveFirstPopupAsync()
        {
            if (_popupNavigation == null || _popupNavigation.PopupStack.Count == 0)
                return Task.CompletedTask;

            PopupPage popupPage = _popupNavigation.PopupStack.FirstOrDefault();

            if (popupPage == null)
                return Task.CompletedTask;

            return RemovePopupPageAsync(popupPage);
        }

        public string CurrentPopupName()
        {
            if (_popupNavigation == null || _popupNavigation.PopupStack.Count == 0)
                return string.Empty;

            PopupPage popupPage = _popupNavigation.PopupStack.FirstOrDefault();
            if (popupPage == null)
                return string.Empty;

            return popupPage.GetType().Name;
        }

        public Task RemovePopupPageAsync(PopupPage popupPage)
        {
            if (popupPage == null)
                return Task.CompletedTask;

            return _popupNavigation?.RemovePageAsync(popupPage);
        }

        #endregion Navigations

        #region Commands

        public ICommand GotoBackCommand
        {
            get
            {
                return new Command<bool?>((useModalNavigation) => GoBackAsync(useModalNavigation));
            }
        }

        /// <summary>
        /// Veri yükle command
        /// </summary>
        public ICommand InitializeCommand => new Command(async () => await InitializeAsync());

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
        private ObservableRangeCollection<TModel> items;

        /// <summary>
        /// Listview içerisine bind edilecek liste
        /// </summary>
        public ObservableRangeCollection<TModel> Items
        {
            get { return items ?? (items = new ObservableRangeCollection<TModel>()); }
        }

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
                }
            }
        }

        /// <summary>
        /// Sayfalarda ortak load işlemleri burada yapılmalı ve refleshs olunca da bu çağrılır
        /// </summary>
        /// <returns></returns>
        protected override Task InitializeAsync()
        {
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

            return base.InitializeAsync();
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

            InitializeCommand.Execute(null);
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

                    if (Items.LastOrDefault().Equals(currentItem))
                        InitializeCommand.Execute(null);
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
