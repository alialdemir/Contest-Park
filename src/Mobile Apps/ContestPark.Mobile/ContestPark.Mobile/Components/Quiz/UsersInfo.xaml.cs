using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersInfo : ContentView
    {
        #region Constructor

        public UsersInfo()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        // Founder

        public static readonly BindableProperty FounderSourceProperty = BindableProperty.Create(propertyName: nameof(FounderSource),
            returnType: typeof(string),
            declaringType: typeof(UsersInfo),
            defaultValue: "#FFC200");

        public string FounderSource
        {
            get => (string)GetValue(FounderSourceProperty);
            set => SetValue(FounderSourceProperty, value);
        }

        public static readonly BindableProperty FounderFullnameProperty = BindableProperty.Create(propertyName: nameof(FounderFullname),
            returnType: typeof(string),
            declaringType: typeof(UsersInfo),
            defaultValue: string.Empty);

        public string FounderFullname
        {
            get => (string)GetValue(FounderFullnameProperty);
            set => SetValue(FounderFullnameProperty, value);
        }

        public static readonly BindableProperty FounderScoreProperty = BindableProperty.Create(propertyName: nameof(FounderScore),
            returnType: typeof(string),
            declaringType: typeof(UsersInfo),
            defaultValue: "0");

        public string FounderScore
        {
            get => (string)GetValue(FounderScoreProperty);
            set => SetValue(FounderScoreProperty, value);
        }

        public static readonly BindableProperty FounderImageBorderColorProperty = BindableProperty.Create(propertyName: nameof(FounderImageBorderColor),
            returnType: typeof(string),
            declaringType: typeof(UsersInfo),
            defaultValue: "#ffc107");

        public string FounderImageBorderColor
        {
            get => (string)GetValue(FounderImageBorderColorProperty);
            set => SetValue(FounderImageBorderColorProperty, value);
        }

        // Opponent

        public static readonly BindableProperty OpponentSourceProperty = BindableProperty.Create(propertyName: nameof(OpponentSource),
            returnType: typeof(string),
            declaringType: typeof(UsersInfo),
            defaultValue: "#FFC200");

        public string OpponentSource
        {
            get => (string)GetValue(OpponentSourceProperty);
            set => SetValue(OpponentSourceProperty, value);
        }

        public static readonly BindableProperty OpponentFullnameProperty = BindableProperty.Create(propertyName: nameof(OpponentFullname),
            returnType: typeof(string),
            declaringType: typeof(UsersInfo),
            defaultValue: string.Empty);

        public string OpponentFullname
        {
            get => (string)GetValue(OpponentFullnameProperty);
            set => SetValue(OpponentFullnameProperty, value);
        }

        public static readonly BindableProperty OpponentScoreProperty = BindableProperty.Create(propertyName: nameof(OpponentScore),
            returnType: typeof(string),
            declaringType: typeof(UsersInfo),
            defaultValue: "0");

        public string OpponentScore
        {
            get => (string)GetValue(OpponentScoreProperty);
            set => SetValue(OpponentScoreProperty, value);
        }

        public static readonly BindableProperty OpponentImageBorderColorProperty = BindableProperty.Create(propertyName: nameof(OpponentImageBorderColor),
            returnType: typeof(string),
            declaringType: typeof(UsersInfo),
            defaultValue: "#ffc107");

        public string OpponentImageBorderColor
        {
            get => (string)GetValue(OpponentImageBorderColorProperty);
            set => SetValue(OpponentImageBorderColorProperty, value);
        }

        // Score

        public static readonly BindableProperty TimeProperty = BindableProperty.Create(propertyName: nameof(Time),
            returnType: typeof(string),
            declaringType: typeof(UsersInfo),
            defaultValue: "10");

        public string Time
        {
            get => (string)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        #endregion Properties
    }
}