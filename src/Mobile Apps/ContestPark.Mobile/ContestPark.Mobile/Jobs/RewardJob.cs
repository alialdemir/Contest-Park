//using ContestPark.Mobile.Services.Notification;
//using Shiny.Jobs;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ContestPark.Mobile.Jobs
//{
//    public class RewardJob : IJob
//    {
//        #region Private variables

//        private readonly INotificationService _notificationService;

//        #endregion Private variables

//        #region Constructor

//        public RewardJob(INotificationService notificationService)
//        {
//            _notificationService = notificationService;
//        }

//        #endregion Constructor

//        #region Properties

//        private bool IsInitialize { get; set; }

//        #endregion Properties

//        #region Methods

//        /// <summary>
//        /// En son aldığı günlük ödülden birsonraki ödülün süresi gelince push notification göndermesi için servere istek atar
//        /// </summary>

//        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
//        {
//            if (IsInitialize)
//            {
//                await _notificationService.PushSendAsync(Enums.PushNotificationTypes.Reward);
//            }

//            //   await _notificationManager.Send("WELCOME", "Houston welcomes you the first ever Xamarin Developer Summit"); // yes, you can see where this was used :)
//            IsInitialize = true;

//            return true;
//        }

//        #endregion Methods
//    }
//}
