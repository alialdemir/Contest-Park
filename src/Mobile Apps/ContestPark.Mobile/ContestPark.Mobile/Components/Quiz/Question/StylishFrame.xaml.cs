using ContestPark.Mobile.Models.Duel;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    
    public partial class StylishFrame : PancakeView
    {
        #region Constructor

        public StylishFrame()
        {
            InitializeComponent();

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) => AnswerCommand?.Execute(Answer);
            GestureRecognizers.Add(tapGestureRecognizer);
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty AnswerProperty = BindableProperty.Create(propertyName: nameof(Answer),
            returnType: typeof(AnswerModel),
            declaringType: typeof(StylishFrame),
            defaultValue: null);

        public AnswerModel Answer
        {
            get => (AnswerModel)GetValue(AnswerProperty);
            set => SetValue(AnswerProperty, value);
        }

        public static readonly BindableProperty AnswerCommandProperty = BindableProperty.Create(propertyName: nameof(AnswerCommand),
            returnType: typeof(Command<AnswerModel>),
            declaringType: typeof(StylishFrame),
            defaultValue: null);

        public Command<AnswerModel> AnswerCommand
        {
            get => (Command<AnswerModel>)GetValue(AnswerCommandProperty);
            set => SetValue(AnswerCommandProperty, value);
        }

        #endregion Properties
    }
}
