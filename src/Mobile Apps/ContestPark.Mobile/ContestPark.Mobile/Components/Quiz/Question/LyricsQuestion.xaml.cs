using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public partial class LyricsQuestion : ContentView
    {
        #region Constructor

        public LyricsQuestion()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties



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


        public static readonly BindableProperty QuestionProperty = BindableProperty.Create(propertyName: nameof(Question),
                                                                                           returnType: typeof(string),
                                                                                           declaringType: typeof(MusicQuestion),
                                                                                           defaultValue: string.Empty);

        public string Question
        {
            get => (string)GetValue(QuestionProperty);
            set => SetValue(QuestionProperty, value);
        }

        public static readonly BindableProperty LinkProperty = BindableProperty.Create(propertyName: nameof(Link),
                                                                                       returnType: typeof(string),
                                                                                       declaringType: typeof(MusicQuestion),
                                                                                       defaultValue: string.Empty);

        public string Link
        {
            get => (string)GetValue(LinkProperty);
            set => SetValue(LinkProperty, value);
        }

        #endregion Properties

        #region Commands

        public Command LyricsScrollViewCommand
        {
            get
            {
                return new Command(async () => await LyricsScrollView.ScrollToAsync(0, 250, false));
            }
        }

        #endregion Commands
    }
}
