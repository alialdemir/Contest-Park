using ContestPark.Mobile.Components.LongPressedEffect;
using ContestPark.Mobile.Droid.CustomRenderer;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("ContestPark")]
[assembly: ExportEffect(typeof(AndroidLongPressedEffect), "LongPressedEffect")]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    public class AndroidLongPressedEffect : PlatformEffect
    {
        private bool _attached;

        public static void Initialize()
        {
        }

        public AndroidLongPressedEffect()
        {
        }

        protected override void OnAttached()
        {
            //because an effect can be detached immediately after attached (happens in listview), only attach the handler one time.
            if (!_attached)
            {
                if (Control != null)
                {
                    Control.LongClickable = true;
                    Control.LongClick += Control_LongClick;

                    Control.Click += Control_Click;
                }
                else
                {
                    Container.LongClickable = true;
                    Container.LongClick += Control_LongClick;
                    Container.Click += Control_Click;
                }
                _attached = true;
            }
        }

        private void Control_Click(object sender, EventArgs e)
        {
            var command = LongPressedEffect.GetSingleTap(Element);
            command?.Execute(LongPressedEffect.GetCommandParameter(Element));
        }

        // Invoke the command if there is one
        private void Control_LongClick(object sender, Android.Views.View.LongClickEventArgs e)
        {
            var command = LongPressedEffect.GetLongPressed(Element);
            command?.Execute(LongPressedEffect.GetCommandParameter(Element));
        }

        protected override void OnDetached()
        {
            if (_attached)
            {
                if (Control != null)
                {
                    Control.LongClickable = true;
                    Control.LongClick -= Control_LongClick;
                    Control.Click -= Control_Click;
                }
                else
                {
                    Container.LongClickable = true;
                    Container.LongClick -= Control_LongClick;
                    Container.Click -= Control_Click;
                }
                _attached = false;
            }
        }
    }
}
