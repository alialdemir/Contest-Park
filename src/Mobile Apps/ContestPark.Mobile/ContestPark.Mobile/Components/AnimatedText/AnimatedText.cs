using System;
using System.Collections.Generic;
using System.Linq;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.VideoPlayer;
using Prism.Events;
using Prism.Ioc;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class AnimatedText : StackLayout
    {
        private readonly SubscriptionToken _videoPlayerOpenedEventSubscriptionToken;
        private readonly SubscriptionToken _duelEndEventsuscriptionToken;
        private readonly DuelEndEvent _duelEndEvent;
        private readonly VideoPlayerOpenedEvent _videoPlayerOpenedEvent;

        public AnimatedText()
        {
            VerticalOptions = LayoutOptions.CenterAndExpand;

            Orientation = StackOrientation.Vertical;
            Spacing = -1;
            IEventAggregator eventAggregator = ContestParkApp
                                                   .Current
                                                   .Container
                                                   .Resolve<IEventAggregator>();

            _duelEndEvent = eventAggregator.GetEvent<DuelEndEvent>();

            _duelEndEventsuscriptionToken = _duelEndEvent
                                               .Subscribe(() => StopAnimation());


            _videoPlayerOpenedEvent = eventAggregator.GetEvent<VideoPlayerOpenedEvent>();

            _videoPlayerOpenedEventSubscriptionToken = _videoPlayerOpenedEvent
                                               .Subscribe(VideoPlaayerEventHandler);
        }

        public static readonly BindableProperty IsVideoRunningProperty =
            BindableProperty.Create(propertyName: nameof(IsVideoRunning),
                                    returnType: typeof(bool),
                                    defaultBindingMode: BindingMode.TwoWay,
                                    declaringType: typeof(YoutubeVideoPlayer),
                                    defaultValue: false);

        public bool IsVideoRunning
        {
            get => (bool)GetValue(IsVideoRunningProperty);
            set => SetValue(IsVideoRunningProperty, value);
        }


        public static readonly BindableProperty IsRunningProperty =
            BindableProperty.Create(nameof(IsRunning),
                                    typeof(bool),
                                    typeof(AnimatedText),
                                    true);

        public static readonly BindableProperty QuestionProperty =
            BindableProperty.Create(propertyName: nameof(Question),
                                    returnType: typeof(string),
                                    declaringType: typeof(AnimatedText),
                                    defaultValue: string.Empty);

        public string Question
        {
            get => (string)GetValue(QuestionProperty);
            set => SetValue(QuestionProperty, value);
        }

        private Animation _animation;


        public static readonly BindableProperty LyricsScrollViewCommandProperty =
            BindableProperty.Create(propertyName: nameof(LyricsScrollViewCommand),
                                    returnType: typeof(Command),
                                    declaringType: typeof(AnimatedText),
                                    defaultValue: null);

        public Command LyricsScrollViewCommand

        {
            get => (Command)GetValue(LyricsScrollViewCommandProperty);
            set => SetValue(LyricsScrollViewCommandProperty, value);
        }

        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        private string AnimationName { get => "AnimatedTextAnimation"; }

        public List<int> PhraseStartDutations
        {
            get
            {
                return new List<int>
                {
                    11810,
14400,
17840,
20480,
23560,
28040,
30060,
30960,
34120,
35280,
37960,
40920,
43640,
46920,
51360,
54520,
57500,
60160,
61380,
64440,
67280,
70160,
73240,
77720,
80120,
83860,
84720,
87760,
90720,
93680,
96680,
10104,
10360,
10728,
13157,
13465,
13765,
14033,
14365,
14789,
15105,
15406,
15686,
15809,
16121,
16389,
16693,
16989,
17425,
17669,
18012,
18201,
18453,
18741,
19041,
19321,
19765,
20029,
20484,
                };

            }
        }

        private List<string> Lyrics { get; set; }

        private int CurrentLyricsIndex { get; set; }

        private string CurrentPhrase
        {
            get { return Lyrics[CurrentLyricsIndex]; }
        }

        private TimeSpan CurrentPhraseDuration
        {
            get
            {
                return TimeSpan.FromMilliseconds(PhraseStartDutations[CurrentLyricsIndex]);
            }
        }

        private void VideoPlaayerEventHandler(VideoPlayerModel videoPlayer)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                InitAnimation("...");

                Device.StartTimer(new TimeSpan(0, 0, 0, 1, 0), () =>
                {
                    TimeSpan currentDuration = videoPlayer.MediaElement.Position;
                   
                    TimeSpan diff = CurrentPhraseDuration - currentDuration;

                    if (diff.Milliseconds < 0)
                    {
                        if (CurrentLyricsIndex >= (Lyrics.Count - 1))
                        {
                            StopAnimation();

                            return false;
                        }

                        InitAnimation(CurrentPhrase);

                        CurrentLyricsIndex += 1;

                        _animation.Commit(this,
                                          AnimationName,
                                          Convert.ToUInt32(CurrentPhrase.Length) * 2,
                                          1300,
                                          Easing.Linear);

                    }

                    return IsRunning;
                });
            });
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsRunning) && IsEnabled)
            {
                //if (IsRunning)
                //    StartAnimation();
                //else
                //    StopAnimation();
                if (!IsRunning)
                    StopAnimation();
            }

            if (propertyName == nameof(Question) && !string.IsNullOrEmpty(Question))
            {
                Lyrics = Question
                    .Split(new string[] { "\n" }, StringSplitOptions.None)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
            }
        }

        private void InitAnimation(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return;

            _animation = new Animation();

            StackLayout sentenceContainer = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 8,
                Spacing = -1
            };

            for (int i = 0; i < phrase.Length; i++)
            {
                char textChar = phrase[i];
                var label = new Label
                {
                    Text = textChar.ToString(),
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    FontSize = 18,
                    TextColor = Color.Black,
                    LineBreakMode = LineBreakMode.WordWrap
                };

                sentenceContainer.Children.Add(label);

                var oneCharAnimationLength = (double)1 / (phrase.Length + 1);

                _animation.Add(i * oneCharAnimationLength, (i + 1)
                    * oneCharAnimationLength,
                    new Animation(_ => label.TextColor = Color.White,
                                  start: 1,
                                  end: 1.75,
                                  easing: Easing.Linear));
            }

            Children.Add(sentenceContainer);

            LyricsScrollViewCommand?.Execute(null);

            if (Children.Count > 2 && Children.Count % 2 == 0)
                Children.RemoveAt(0);
        }

        private void StopAnimation()
        {
            this.AbortAnimation(AnimationName);

            _duelEndEvent.Unsubscribe(_duelEndEventsuscriptionToken);

            _videoPlayerOpenedEvent.Unsubscribe(_videoPlayerOpenedEventSubscriptionToken);
        }
    }
}