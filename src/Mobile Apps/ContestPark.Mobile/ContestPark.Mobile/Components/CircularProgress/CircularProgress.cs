using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class CircularProgress : View
    {
        #region Properties

        public static readonly BindableProperty IndeterminateProperty = BindableProperty.Create(propertyName: nameof(Indeterminate),
           returnType: typeof(bool),
           declaringType: typeof(CircularProgress),
           defaultValue: true);

        public bool Indeterminate
        {
            get => (bool)GetValue(IndeterminateProperty);
            set => SetValue(IndeterminateProperty, value);
        }

        public static readonly BindableProperty ProgressProperty = BindableProperty.Create(propertyName: nameof(Progress),
            returnType: typeof(float),
            declaringType: typeof(CircularProgress),
            defaultValue: default(float));

        /// <summary>
        /// Gets or sets the current progress
        /// </summary>
        /// <value>The progress.</value>
        public float Progress
        {
            get => (float)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, (float)value);
        }

        public static readonly BindableProperty MaxProperty = BindableProperty.Create(propertyName: nameof(MaxProperty),
            returnType: typeof(float),
            declaringType: typeof(CircularProgress),
            defaultValue: default(float));

        /// <summary>
        /// Gets or sets the max value
        /// </summary>
        /// <value>The max.</value>
        public float Max
        {
            get => (float)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public static readonly BindableProperty ProgressBackgroundColorProperty = BindableProperty.Create(propertyName: nameof(ProgressBackgroundColor),
            returnType: typeof(Color),
            declaringType: typeof(CircularProgress),
            defaultValue: Color.White);

        /// <summary>
        /// Gets or sets the ProgressBackgroundColorProperty
        /// </summary>
        /// <value>The color of the ProgressBackgroundColorProperty.</value>
        public Color ProgressBackgroundColor
        {
            get => (Color)GetValue(ProgressBackgroundColorProperty);
            set => SetValue(ProgressBackgroundColorProperty, value);
        }

        public static readonly BindableProperty ProgressColorProperty = BindableProperty.Create(propertyName: nameof(ProgressColor),
            returnType: typeof(Color),
            declaringType: typeof(CircularProgress),
            defaultValue: Color.Red);

        /// <summary>
        /// Gets or sets the progress color
        /// </summary>
        /// <value>The color of the progress.</value>
        public Color ProgressColor
        {
            get => (Color)GetValue(ProgressColorProperty);
            set => SetValue(ProgressColorProperty, value);
        }

        public static readonly BindableProperty IndeterminateSpeedProperty = BindableProperty.Create(propertyName: nameof(IndeterminateSpeed),
            returnType: typeof(int),
            declaringType: typeof(CircularProgress),
            defaultValue: 100);

        public int IndeterminateSpeed
        {
            get => (int)GetValue(IndeterminateSpeedProperty);
            set => SetValue(IndeterminateSpeedProperty, value);
        }

        #endregion Properties
    }
}