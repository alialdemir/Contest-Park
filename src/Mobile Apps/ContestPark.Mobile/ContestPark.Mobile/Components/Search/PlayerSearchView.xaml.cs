using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Services.Follow;
using Prism.Ioc;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components.Search
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayerSearchView : ContentView
    {
        #region Constructor

        public PlayerSearchView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        private ICommand followOrUnFollowCommand;

        public ICommand FollowOrUnFollowCommand
        {
            get
            {
                return followOrUnFollowCommand ?? (followOrUnFollowCommand = new Command(async () => await ExecuteFollowOrUnFollowCommandAsync()));
            }
        }

        public ICommand GotoProfilePageCommand
        {
            set
            {
                cstmGrid.SingleTap = imgProfilePicture.Command = value;
            }
        }

        public bool IsBusy { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Kullanıcı takip et takibi bırak
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteFollowOrUnFollowCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            IFollowService followService = RegisterTypesConfig.Container.Resolve<IFollowService>();
            SearchModel searchModel = (SearchModel)BindingContext;

            if (followService == null || searchModel == null)
                return;

            bool isSuccesss = await (searchModel.IsFollow == true ?
                  followService.UnFollowAsync(searchModel.UserId) :
                  followService.FollowUpAsync(searchModel.UserId));

            if (isSuccesss)
            {
                ((SearchModel)BindingContext).IsFollow = !searchModel.IsFollow;
            }

            IsBusy = false;
        }

        #endregion Methods
    }
}