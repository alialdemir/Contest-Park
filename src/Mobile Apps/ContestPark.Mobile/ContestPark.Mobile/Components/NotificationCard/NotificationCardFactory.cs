using ContestPark.Mobile.Models.Notification;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{

    public class NotificationCardFactory : ContentView
    {
        #region Constructor

        public NotificationCardFactory()
        {
            Content = CreateContent(new UserFollowListItem());// Skeleton loading için eklendi
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty GotoProfilePageCommandProperty = BindableProperty.Create(propertyName: nameof(GotoProfilePageCommand),
                                                                                                         returnType: typeof(ICommand),
                                                                                                         declaringType: typeof(NotificationCardFactory),
                                                                                                         defaultValue: null);

        public static readonly BindableProperty FollowProcessCommandProperty = BindableProperty.Create(propertyName: nameof(FollowProcessCommand),
                                                                                                     returnType: typeof(ICommand),
                                                                                                     declaringType: typeof(NotificationCardFactory),
                                                                                                     defaultValue: null);

        public static readonly BindableProperty GotoPostDetailCommandProperty = BindableProperty.Create(propertyName: nameof(GotoPostDetailCommand),
                                                                                                     returnType: typeof(ICommand),
                                                                                                     declaringType: typeof(NotificationCardFactory),
                                                                                                     defaultValue: null);

        public ICommand GotoPostDetailCommand
        {
            get { return (ICommand)GetValue(GotoPostDetailCommandProperty); }
            set
            {
                SetValue(GotoPostDetailCommandProperty, value);
            }
        }

        public ICommand GotoProfilePageCommand
        {
            get { return (ICommand)GetValue(GotoProfilePageCommandProperty); }
            set
            {
                SetValue(GotoProfilePageCommandProperty, value);
            }
        }

        public ICommand FollowProcessCommand
        {
            get { return (ICommand)GetValue(FollowProcessCommandProperty); }
            set
            {
                SetValue(FollowProcessCommandProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            NotificationModel notificationModel = (NotificationModel)BindingContext;
            if (notificationModel == null)
                return;

            switch (notificationModel.NotificationType)
            {
                case Enums.NotificationTypes.Follow:
                    Content = CreateContent(new UserFollowListItem()
                    {
                        BindingContext = BindingContext,
                        GotoProfilePageCommand = GotoProfilePageCommand,
                        RightButtonCommand = FollowProcessCommand
                    });
                    break;

                default:
                    Content = CreateContent(new RightThumListItem()
                    {
                        BindingContext = BindingContext,
                        GotoProfilePageCommand = GotoProfilePageCommand,
                        GestureRecognizers =
                        {
                            new TapGestureRecognizer
                            {
                                Command = GotoPostDetailCommand,
                                CommandParameter = notificationModel.PostId,
                            }
                        }
                    });
                    break;
            }
        }

        private StackLayout CreateContent(ContentView view)
        {
            return new StackLayout
            {
                Margin = new Thickness(0, 0, 0, 8),
                Children =
                {
                    new Frame
                    {
                        Padding = 0,
                        HasShadow = true,
                        IsClippedToBounds = true,
                        CornerRadius = 8,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        BackgroundColor = (Color)Application.Current.Resources["White"],
                        Content = new StackLayout
                        {
                            Spacing = 0,
                            Children =
                            {
                                view,
                            }
                        }
                    }
                }
            };
        }

        #endregion Methods
    }
}
