using ContestPark.Mobile.Models.Slide;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    
    public partial class NoticeView : ContentView
    {
        #region Constructor

        public NoticeView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty NoticesProperty = BindableProperty.Create(propertyName: nameof(Notices),
            propertyChanged: NoticesProperyChanged,
                                                                                          returnType: typeof(IEnumerable<NoticeModel>),
                                                                                          declaringType: typeof(NoticeView),
                                                                                          defaultValue: null);

        public IEnumerable<NoticeModel> Notices
        {
            get { return (IEnumerable<NoticeModel>)GetValue(NoticesProperty); }
            set { SetValue(NoticesProperty, value); }
        }

        public static readonly BindableProperty NoticeNavigateToCommandProperty = BindableProperty.Create(propertyName: nameof(NoticeNavigateToCommand),
                                                                                                          returnType: typeof(ICommand),
                                                                                                          declaringType: typeof(NoticeView),
                                                                                                          defaultValue: null);

        /// <summary>
        /// Alt kategori display alert command
        /// </summary>
        public ICommand NoticeNavigateToCommand
        {
            get { return (ICommand)GetValue(NoticeNavigateToCommandProperty); }
            set { SetValue(NoticeNavigateToCommandProperty, value); }
        }

        #endregion Properties

        #region Events

        private static void NoticesProperyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            List<NoticeModel> notices = (List<NoticeModel>)newValue;
            if (notices == null || !notices.Any())
            {
                ((NoticeView)bindable).HeightRequest = 0;
            }
        }

        #endregion Overrides
    }
}
