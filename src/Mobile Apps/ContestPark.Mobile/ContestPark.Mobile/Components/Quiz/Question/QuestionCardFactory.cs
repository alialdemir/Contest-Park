using ContestPark.Mobile.Enums;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class QuestionCardFactory : ContentView
    {
        #region Properties

        public static readonly BindableProperty QuestionProperty = BindableProperty.Create(propertyName: nameof(Question),
                                                                                           returnType: typeof(string),
                                                                                           declaringType: typeof(QuestionCardFactory),
                                                                                           defaultValue: string.Empty);

        public string Question
        {
            get => (string)GetValue(QuestionProperty);
            set => SetValue(QuestionProperty, value);
        }

        public static readonly BindableProperty LinkProperty = BindableProperty.Create(propertyName: nameof(Link),
                                                                                       returnType: typeof(string),
                                                                                       declaringType: typeof(QuestionCardFactory),
                                                                                       defaultValue: string.Empty);

        public string Link
        {
            get => (string)GetValue(LinkProperty);
            set => SetValue(LinkProperty, value);
        }

        public static readonly BindableProperty QuestionTypeProperty = BindableProperty.Create(propertyName: nameof(QuestionType),
                                                                                               returnType: typeof(QuestionTypes),
                                                                                               declaringType: typeof(QuestionCardFactory),
                                                                                               defaultValue: QuestionTypes.None);

        public QuestionTypes QuestionType
        {
            get => (QuestionTypes)GetValue(QuestionTypeProperty);
            set => SetValue(QuestionTypeProperty, value);
        }

        #endregion Properties

        #region Override

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(QuestionType) || propertyName == nameof(Question) || propertyName == nameof(Link))
            {
                switch (QuestionType)
                {
                    case QuestionTypes.Music:
                        Content = new MusicQuestion() { Question = Question, Link = Link }; break;

                    case QuestionTypes.Lyrics:
                        Content = new LyricsQuestion() { Question = Question, Link = Link }; break;

                    case QuestionTypes.Image:
                        Content = new ImageQuestion() { Question = Question, Link = Link }; break;

                    case QuestionTypes.Text:
                        Content = new TextQuestion() { Question = Question }; break;
                }
            }
        }

        #endregion Override
    }
}
