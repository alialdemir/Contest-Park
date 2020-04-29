using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MusicQuestion : ContentView
    {
        #region Constructor

        public MusicQuestion()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

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

        //#region Commands

        //public ICommand WindBackCommand
        //{
        //    get
        //    {
        //        return new Command(() =>
        //        {
        //            IAudioService audioService = ContestParkApp.Current.Container.Resolve<IAudioService>();
        //            if (audioService == null)
        //                return;

        //            audioService.WindBack();
        //        });
        //    }
        //}

        //public ICommand PlayCommand
        //{
        //    get
        //    {
        //        return new Command(() =>
        //        {
        //            IAudioService audioService = ContestParkApp.Current.Container.Resolve<IAudioService>();
        //            if (audioService == null || string.IsNullOrEmpty(Link))
        //                return;

        //            audioService.ToggleAudio(Link);
        //        });
        //    }
        //}

        //public ICommand FastForwardCommand
        //{
        //    get
        //    {
        //        return new Command(() =>
        //        {
        //            IAudioService audioService = ContestParkApp.Current.Container.Resolve<IAudioService>();
        //            if (audioService == null)
        //                return;

        //            audioService.FastForward();
        //        });
        //    }
        //}

        //#endregion Commands
    }
}
