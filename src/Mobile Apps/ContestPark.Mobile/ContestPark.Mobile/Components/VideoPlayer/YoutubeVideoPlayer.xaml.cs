using System.Runtime.CompilerServices;
using Acr.UserDialogs;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace ContestPark.Mobile.Components
{
    public partial class YoutubeVideoPlayer : ContentView
    {
        public YoutubeVideoPlayer()
        {
            InitializeComponent();

            MyMediaElement.SeekCompleted += MyMediaElement_SeekCompleted;
            MyMediaElement.MediaOpened += MyMediaElement_MediaOpened;
        }

        public static readonly BindableProperty IsVideoRunningProperty = BindableProperty.Create(propertyName: nameof(IsVideoRunning),
                                                                                  returnType: typeof(bool),
                                                                                  defaultBindingMode: BindingMode.TwoWay,
                                                                                  declaringType: typeof(YoutubeVideoPlayer),
                                                                                  defaultValue: false);

        public  bool IsVideoRunning
        {
            get => (bool)GetValue(IsVideoRunningProperty);
            set => SetValue(IsVideoRunningProperty, value);
        }

        public static readonly BindableProperty LinkProperty = BindableProperty.Create(propertyName: nameof(Link),
                                                                                  returnType: typeof(string),
                                                                                  declaringType: typeof(YoutubeVideoPlayer),
                                                                                  defaultValue: string.Empty);

        public string Link
        {
            get => (string)GetValue(LinkProperty);
            set => SetValue(LinkProperty, value);
        }

        public bool IsBusy { get; set; }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(Link) || !string.IsNullOrEmpty(Link))
            {
                GetVideoContent();
            }
        }

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

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(Link);

            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
            if (streamInfo != null)
            {
                // Get the actual stream
            //    var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                MyMediaElement.Source = streamInfo.Url;
            }

            

            IsBusy = false;
        }

        private void MyMediaElement_MediaOpened(object sender, System.EventArgs e)
        {
            IsVideoRunning = true;

            UserDialogs.Instance.HideLoading();
        }

        private void MyMediaElement_SeekCompleted(object sender, System.EventArgs e)
        {
        }

        void MediaElement_MediaOpened(System.Object sender, System.EventArgs e)
        {
            // MyActivityIndicator.IsVisible = false;
        }
    }
}