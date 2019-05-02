using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Converters;
using ContestPark.Mobile.Models.Chat;
using ContestPark.Mobile.Services.Settings;
using Prism.Ioc;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components.ChatDetail
{
    public class ChatDetail : ContentView
    {
        #region Methods

        private void CreateListItem()
        {
            var Model = (ChatDetailModel)BindingContext;
            if (Model == null)
                return;

            ISettingsService settingsService = RegisterTypesConfig.Container.Resolve<ISettingsService>();
            if (settingsService == null)
                return;

            bool isSenderUser = !(Model.SenderId == settingsService.CurrentUser.UserId);//Mesajı gönderen karşı taraf ise true

            #region Grid info

            Grid grid = new Grid()
            {
                Padding = new Thickness(10),
                RowSpacing = 0
            };
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            // Column definitions
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = isSenderUser ? 0 : new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = !isSenderUser ? 0 : new GridLength(1, GridUnitType.Star) });

            #endregion Grid info

            #region Color and LayoutOptions settings

            Color textColor = (Color)(isSenderUser ? Application.Current.Resources["DarkGray"] : Application.Current.Resources["White"]);
            Color messgeBackgroundColor = (Color)(isSenderUser ? Application.Current.Resources["Gray"] : Application.Current.Resources["Blue"]);
            LayoutOptions lyoutOptions = isSenderUser ? LayoutOptions.StartAndExpand : LayoutOptions.EndAndExpand;

            #endregion Color and LayoutOptions settings

            #region Label message

            HtmlLabel lblMessage = new HtmlLabel()
            {
                FontSize = 16,
                Text = Model.Message,
                LineBreakMode = LineBreakMode.WordWrap,
                TextColor = textColor,
            };

            #endregion Label message

            #region Label date

            Label lblDate = new Label()
            {
                FontSize = 13,
                TextColor = textColor,
                Text = DateTimeMomentConverter.GetPrettyDate(Model.Date),
            };

            #endregion Label date

            #region Root stack

            StackLayout stack = new StackLayout()
            {
                Spacing = 0,

                Padding = new Thickness(5),
                HorizontalOptions = lyoutOptions,
                BackgroundColor = messgeBackgroundColor,
                Children =
                    {
                        lblMessage,
                        lblDate
                    }
            };

            #endregion Root stack

            #region Left or right

            if (isSenderUser)
            {
                grid.Children.Add(stack, 1, 0);
            }
            else
            {
                grid.Children.Add(stack, 0, 0);
            }

            #endregion Left or right

            Content = grid;
        }

        #endregion Methods

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            CreateListItem();
        }

        #endregion Override
    }
}