using ContestPark.Mobile.Components.PostCardView;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Post;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class PostCardFactoryView : ContentView
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

            PostModel postListModel = (PostModel)BindingContext;
            if (postListModel != null)
            {
                switch (postListModel.PostType)
                {
                    case PostTypes.ContestDuel:
                        Content = CreateContent(new ContestPostCard(NavigationService));
                        break;

                    case PostTypes.Follow:
                        Content = CreateContent(new FollowPostCard(NavigationService));
                        break;

                    case PostTypes.ProfilePictureChanged:
                    case PostTypes.CoverPictureChanged:
                        Content = CreateContent(new ImagePostCard(NavigationService));
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
                                new BottomPostCard(NavigationService)
                            }
                        }
                    }
                }
            };
        }
    }
}