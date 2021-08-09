using System.Threading.Tasks;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Quiz;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{

    public partial class Stylish : ContentView
    {
        #region Constructor

        public Stylish()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty AnswerTypeProperty = BindableProperty.Create(propertyName: nameof(AnswerType),
            returnType: typeof(AnswerTypes),
            declaringType: typeof(Stylish),
            defaultValue: AnswerTypes.Text);

        public AnswerTypes AnswerType
        {
            get => (AnswerTypes)GetValue(AnswerTypeProperty);
            set => SetValue(AnswerTypeProperty, value);
        }

        public static readonly BindableProperty AnswersProperty = BindableProperty.Create(propertyName: nameof(Answers),
            returnType: typeof(AnswerPair),
            declaringType: typeof(Stylish),
            defaultValue: new AnswerPair(null, null, null, null));

        public AnswerPair Answers
        {
            get => (AnswerPair)GetValue(AnswersProperty);
            set => SetValue(AnswersProperty, value);
        }

        public static readonly BindableProperty AnswerCommandProperty = BindableProperty.Create(propertyName: nameof(AnswerCommand),
            returnType: typeof(Command<AnswerModel>),
            declaringType: typeof(Stylish),
            defaultValue: null);

        public Command<AnswerModel> AnswerCommand
        {
            get => (Command<AnswerModel>)GetValue(AnswerCommandProperty);
            set => SetValue(AnswerCommandProperty, value);
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Şıkları animasyon yapar
        /// </summary>
        public async void AnimateStylish()
        {
            Stylish1.Opacity = 0;
            Stylish2.Opacity = 0;
            Stylish3.Opacity = 0;
            Stylish4.Opacity = 0;

            await Task.Delay(500);
            await Stylish1.FadeTo(1, 600, Easing.CubicOut);
            await Stylish2.FadeTo(1, 700, Easing.CubicOut);
            await Stylish3.FadeTo(1, 800, Easing.CubicOut);
            await Stylish4.FadeTo(1, 900, Easing.CubicOut);
        }

        #endregion Methods
    }
}