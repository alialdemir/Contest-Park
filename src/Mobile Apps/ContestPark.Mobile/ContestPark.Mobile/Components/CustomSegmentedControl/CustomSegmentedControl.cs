using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    /// <summary>
    /// SegmentedControl Interface
    /// https://github.com/alexrainman/SegmentedControl
    /// </summary>
    public class CustomSegmentedControl : View, IViewContainer<SegmentedControlOption>
    {
        public IList<SegmentedControlOption> Children { get; set; }

        public CustomSegmentedControl()
        {
            Children = new List<SegmentedControlOption>();
            ValueChanged += (sender, e) => ValueChangedCommand?.Execute(((CustomSegmentedControl)sender).SelectedSegment);
        }

        // PCL event handler
        public event ValueChangedEventHandler ValueChanged;

        public delegate void ValueChangedEventHandler(object sender, EventArgs e);

        public static readonly BindableProperty ValueChangedCommandProperty = BindableProperty.Create(propertyName: nameof(ValueChangedCommand),
                                                                                                      returnType: typeof(ICommand),
                                                                                                      declaringType: typeof(CustomSegmentedControl),
                                                                                                      defaultValue: null);

        public ICommand ValueChangedCommand
        {
            get { return (ICommand)GetValue(ValueChangedCommandProperty); }
            set
            {
                SetValue(ValueChangedCommandProperty, value);
            }
        }

        public static readonly BindableProperty SelectedSegmentBindableProperty = BindableProperty.Create(propertyName: nameof(SelectedSegment),
                                                                                                          returnType: typeof(int),
                                                                                                          declaringType: typeof(CustomSegmentedControl),
                                                                                                          defaultValue: 0);

        public int SelectedSegment
        {
            get
            {
                return (int)GetValue(SelectedSegmentBindableProperty);
            }
            set
            {
                SetValue(SelectedSegmentBindableProperty, value);
            }
        }

        private string _selectedText;

        public string SelectedText
        {
            get
            {
                return _selectedText;
            }
            set
            {
                _selectedText = value;
                if (ValueChanged != null)
                    ValueChanged(this, EventArgs.Empty);
            }
        }

        public static readonly BindableProperty IsSelectedCheckBindableProperty = BindableProperty.Create(propertyName: nameof(IsSelectedCheck),
                                                                                                          returnType: typeof(bool),
                                                                                                          declaringType: typeof(CustomSegmentedControl),
                                                                                                          defaultValue: false);

        /// <summary>
        /// Tıklandığında checked olsun mu true olsun false olmasın default false
        /// </summary>
        public bool IsSelectedCheck
        {
            get { return (bool)GetValue(IsSelectedCheckBindableProperty); }
            set { SetValue(IsSelectedCheckBindableProperty, value); }
        }

        public static readonly BindableProperty TintColorBindableProperty = BindableProperty.Create(propertyName: nameof(TintColor),
                                                                                                          returnType: typeof(Color),
                                                                                                          declaringType: typeof(CustomSegmentedControl),
                                                                                                          defaultValue: Color.FromHex("#007AFF"));

        public Color TintColor
        {
            get { return (Color)GetValue(TintColorBindableProperty); }
            set { SetValue(TintColorBindableProperty, value); }
        }

        public static readonly BindableProperty SelectedTextColorBindableProperty = BindableProperty.Create(propertyName: nameof(SelectedTextColor),
                                                                                                          returnType: typeof(Color),
                                                                                                          declaringType: typeof(CustomSegmentedControl),
                                                                                                          defaultValue: Color.FromHex("#FFFFFF"));

        public Color SelectedTextColor
        {
            get { return (Color)GetValue(SelectedTextColorBindableProperty); }
            set { SetValue(SelectedTextColorBindableProperty, value); }
        }

        public Action<int> SelectTabAction;

        public void SelectTab(int position)
        {
            if (SelectTabAction != null)
                SelectTabAction(position);
        }

        public Action<Color> SetTintColorAction;

        public void SetTintColor(Color color)
        {
            if (SetTintColorAction != null)
                SetTintColorAction(color);
        }

        public Action<bool> SetIsSelectedCheckAction;

        public void SetIsSelectedCheck(bool color)
        {
            if (SetIsSelectedCheckAction != null)
                SetIsSelectedCheckAction(color);
        }
    }

    public class SegmentedControlOption : View
    {
        public static readonly BindableProperty TextBindableProperty = BindableProperty.Create(propertyName: nameof(Text),
                                                                                                          returnType: typeof(string),
                                                                                                          declaringType: typeof(SegmentedControlOption),
                                                                                                          defaultValue: String.Empty);

        public string Text
        {
            get { return (string)GetValue(TextBindableProperty); }
            set { SetValue(TextBindableProperty, value); }
        }
    }
}