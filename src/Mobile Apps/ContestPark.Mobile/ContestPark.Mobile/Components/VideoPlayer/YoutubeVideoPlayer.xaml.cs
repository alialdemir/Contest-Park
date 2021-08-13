using System.Runtime.CompilerServices;
using Acr.UserDialogs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.VideoPlayer;
using Prism.Events;
using Prism.Ioc;
using Xamarin.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace ContestPark.Mobile.Components
{
    public partial class YoutubeVideoPlayer : ContentView
    {
        private readonly IEventAggregator _eventAggregator;
        public YoutubeVideoPlayer()
        {
            InitializeComponent();

            _eventAggregator = ContestParkApp
                                              .Current
                                              .Container
                                              .Resolve<IEventAggregator>();
        }

        public static readonly BindableProperty IsVideoRunningProperty = BindableProperty.Create(propertyName: nameof(IsVideoRunning),
                                                                                                 returnType: typeof(bool),
                                                                                                 defaultBindingMode: BindingMode.TwoWay,
                                                                                                 declaringType: typeof(YoutubeVideoPlayer),
                                                                                                 defaultValue: false);

        public bool IsVideoRunning
        {
            get => (bool)GetValue(IsVideoRunningProperty);
            set => SetValue(IsVideoRunningProperty, value);
        }

        public static readonly BindableProperty YoutubeVideoIdProperty = BindableProperty.Create(propertyName: nameof(YoutubeVideoId),
                                                                                                 returnType: typeof(string),
                                                                                                 declaringType: typeof(YoutubeVideoPlayer),
                                                                                                 defaultValue: string.Empty);

        public string YoutubeVideoId
        {
            get => (string)GetValue(YoutubeVideoIdProperty);
            set => SetValue(YoutubeVideoIdProperty, value);
        }

        public bool IsBusy { get; set; }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(YoutubeVideoId) || !string.IsNullOrEmpty(YoutubeVideoId))
            {
                GetVideoContent();
            }
        }

        /// <summary>
        /// Youtube video id ile videoyu media elemente yükler
        /// </summary>
        private async void GetVideoContent()
        {
            if (IsBusy || MyMediaElement.Source != null)
                return;

            IsBusy = true;

            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            var youtube = new YoutubeClient();

            //// You can specify video ID or URL
            //var video = await youtube.Videos.GetAsync($"https://www.youtube.com/watch?v={Link}&t=15s");

            //var title = video.Title; // "Downloaded Video Title"
            //var author = video.Author; // "Downloaded Video Author"
            //var duration = video.Duration; // "Downloaded Video Duration Count"

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(YoutubeVideoId);

            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
            if (streamInfo != null)
            {
                // Get the actual stream
                //    var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                MyMediaElement.Source = streamInfo.Url;
            }

            IsBusy = false;
        }

        /// <summary>
        /// Video başlatıldığında çalışır ve VideoPlayerOpenedEvent public eder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaOpenedEventHandle(object sender, System.EventArgs e)
        {
            _eventAggregator
                .GetEvent<VideoPlayerOpenedEvent>()
                .Publish(new VideoPlayerModel
                {
                    MediaElement = MyMediaElement
                });

            IsVideoRunning = true;

            UserDialogs.Instance.HideLoading();
        }

        /// <summary>
        /// Videp hataya uğradığında çalışır ve MediaFailedEvent public eder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaFailedEventHandle(object sender, System.EventArgs e)
        {
            _eventAggregator
                .GetEvent<MediaFailedEvent>()
                .Publish(YoutubeVideoId);

            IsVideoRunning = false;
        }

        /// <summary>
        /// Video sonlandığında çalışır ve MediaEndedEvent public eder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaEndedEventHandle(object sender, System.EventArgs e)
        {
            _eventAggregator
                .GetEvent<MediaEndedEvent>()
                .Publish(YoutubeVideoId);

            IsVideoRunning = false;
        }
    }
}