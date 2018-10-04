using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RankView : ContentView
    {
        #region Constructors

        public RankView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Property

        public static readonly BindableProperty RankProperty = BindableProperty.Create(propertyName: nameof(Rank),
                                                                                                returnType: typeof(string),
                                                                                                declaringType: typeof(RankView),
                                                                                                defaultValue: String.Empty);

        public string Rank
        {
            get { return (string)GetValue(RankProperty); }
            set { SetValue(RankProperty, value); }
        }

        public static readonly BindableProperty GotoProfilePageCommandProperty = BindableProperty.Create(propertyName: nameof(GotoProfilePageCommand),
                                                                                           returnType: typeof(ICommand),
                                                                                           declaringType: typeof(ListItemView),
                                                                                           defaultValue: null);

        public ICommand GotoProfilePageCommand
        {
            get { return (ICommand)GetValue(GotoProfilePageCommandProperty); }
            set
            {
                SetValue(GotoProfilePageCommandProperty, value);
            }
        }

        #endregion Property
    }
}