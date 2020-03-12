using ContestPark.Mobile.Models.Notification;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class NotificationCardFactory : ContentView
    {
        public static readonly BindableProperty NavigationServiceProperty = BindableProperty.Create(propertyName: nameof(NavigationService),
                                                                                              returnType: typeof(INavigationService),
                                                                                              declaringType: typeof(PostCardFactoryView),
                                                                                              defaultValue: null);

        public INavigationService NavigationService
        {
            get { return (INavigationService)GetValue(NavigationServiceProperty); }
            set { SetValue(NavigationServiceProperty, value); }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            NotificationModel notificationModel = (NotificationModel)BindingContext;
            if (notificationModel != null)
            {
                switch (notificationModel.NotificationType)
                {
                    case Enums.NotificationTypes.Follow:
                        Content = CreateContent(new UserFollowListItem());
                        break;
                }
            }
        }

        private StackLayout CreateContent(ContentView view)
        {
            return new StackLayout
            {
                Margin = 8,
                Children =
                {
                    new Frame
                    {
                        Padding = 0,
                        HasShadow = true,
                        IsClippedToBounds = true,
                        CornerRadius = 10,
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
    }
}
