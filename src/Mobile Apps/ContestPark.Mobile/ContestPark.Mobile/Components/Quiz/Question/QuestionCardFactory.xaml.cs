using ContestPark.Mobile.Enums;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionCardFactory : ContentView
    {
        #region Constructor

        public QuestionCardFactory()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty QuestionProperty = BindableProperty.Create(propertyName: nameof(Question),
                                                                                           returnType: typeof(string),
                                                                                           declaringType: typeof(QuestionCardFactory),
                                                                                           defaultValue: string.Empty);

        public string Question
        {
            get
            {
                ChangeImageVisible();

                return (string)GetValue(QuestionProperty);
            }
            set => SetValue(QuestionProperty, value);
        }

        private void ChangeImageVisible()
        {
            if (img == null)
                return;

            switch (QuestionType)
            {
                case QuestionTypes.Image: img.IsVisible = true; break;
                case QuestionTypes.Text: img.IsVisible = false; break;
            }
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
                                                                                               defaultValue: QuestionTypes.Text);

        public QuestionTypes QuestionType
        {
            get => (QuestionTypes)GetValue(QuestionTypeProperty);
            set => SetValue(QuestionTypeProperty, value);
        }

        #endregion Properties
    }
}