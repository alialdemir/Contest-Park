using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    
    public partial class TextQuestion : ContentView
    {
        #region Constructor

        public TextQuestion()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty QuestionProperty = BindableProperty.Create(propertyName: nameof(Question),
                                                                                           returnType: typeof(string),
                                                                                           declaringType: typeof(TextQuestion),
                                                                                           defaultValue: string.Empty);

        public string Question
        {
            get => (string)GetValue(QuestionProperty);
            set => SetValue(QuestionProperty, value);
        }

        #endregion Properties
    }
}
