using System.Collections.Generic;
using System.Linq;
using ContestPark.Mobile.Events;
using Prism.Events;
using Prism.Ioc;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class AnimatedText : StackLayout
    {
        private readonly SubscriptionToken _duelEndEventsubscriptionToken;
        private readonly DuelEndEvent _duelEndEvent;
        int _stackLayoutIndex = 1;


        public AnimatedText()
        {
            VerticalOptions = LayoutOptions.CenterAndExpand;

            Orientation = StackOrientation.Vertical;
            Spacing = -1;


            _duelEndEvent = ContestParkApp
                                        .Current
                                        .Container
                                        .Resolve<IEventAggregator>()
                                        .GetEvent<DuelEndEvent>();

            _duelEndEventsubscriptionToken = _duelEndEvent
                                               .Subscribe(() => StopAnimation());
        }

        private const string AnimationName = "AnimatedTextAnimation";


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


        public static readonly BindableProperty IsRunningProperty =
            BindableProperty.Create(nameof(IsRunning),
                                    typeof(bool),
                                    typeof(AnimatedText),
                                    default(bool));

        public static readonly BindableProperty QuestionProperty = BindableProperty.Create(propertyName: nameof(Question),
                                                                                           returnType: typeof(string),
                                                                                           declaringType: typeof(AnimatedText),
                                                                                           defaultValue: string.Empty);

        public string Question
        {
            get => (string)GetValue(QuestionProperty);
            set => SetValue(QuestionProperty, value);
        }

        private Animation _animation;


        public static readonly BindableProperty LyricsScrollViewCommandProperty = BindableProperty.Create(propertyName: nameof(LyricsScrollViewCommand),
                                                                                                          returnType: typeof(Command),
                                                                                                          declaringType: typeof(AnimatedText),
                                                                                                          defaultValue: null);

        public Command LyricsScrollViewCommand

        {
            get => (Command)GetValue(LyricsScrollViewCommandProperty);
            set => SetValue(LyricsScrollViewCommandProperty, value);
        }


        List<string> lyrics = new List<string>
        {
        };

        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsRunning) && IsEnabled)
            {
                if (IsRunning)
                    StartAnimation();
                else
                    StopAnimation();
            }

            if (propertyName == nameof(Question) && !string.IsNullOrEmpty(Question))
            {
                lyrics = Question
                    .Split(new string[] { "\n" }, System.StringSplitOptions.None)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();

                _stackLayoutIndex = lyrics.Count - 1;
            }
        }

        private void InitAnimation(string item)
        {
            _animation = new Animation();

            //    Children.Clear();

            if (string.IsNullOrWhiteSpace(item))
                return;

            var index = 0;
            StackLayout sentenceContainer = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 8,
                Spacing = -1
            };
            foreach (var textChar in item)
            {
                var label = new Label
                {
                    Text = textChar.ToString(),
                    HorizontalOptions = LayoutOptions.Fill,
                    FontSize = 22,
                    TextColor = Color.Black,
                    LineBreakMode = LineBreakMode.WordWrap
                };

                sentenceContainer.Children.Add(label);

                var oneCharAnimationLength = (double)1 / (item.Length + 1);

                _animation.Add(index * oneCharAnimationLength, (index + 1) * oneCharAnimationLength,
                    new Animation(v => label.TextColor = Color.White, 1, 1.75, Easing.Linear));

                index++;

                Children.Add(sentenceContainer);
            }

            if (Children.Count > 2 && Children.Count % 2 == 0)
                Children.RemoveAt(0);
        }

        private void StartAnimation()
        {
            if (_stackLayoutIndex < 0)
                return;

            InitAnimation(lyrics[_stackLayoutIndex]);

            _stackLayoutIndex -= 1;

            _animation.Commit(this, AnimationName, 16, 7000,
                Easing.Linear,
                (_, __) =>
                {
                    StartAnimation();

                    LyricsScrollViewCommand?.Execute(null);
                }, () => false);
        }

        private void StopAnimation()
        {
            this.AbortAnimation(AnimationName);

            _stackLayoutIndex = -1;

            _duelEndEvent.Unsubscribe(_duelEndEventsubscriptionToken);
        }
    }
}