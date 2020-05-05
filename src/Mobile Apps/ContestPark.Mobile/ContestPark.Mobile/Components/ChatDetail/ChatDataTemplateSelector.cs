using ContestPark.Mobile.Models.Chat;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components.ChatDetail
{
    public class ChatDataTemplateSelector : DataTemplateSelector

    {
        public ChatDataTemplateSelector()
        {
            // Retain instances!
            this.incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.outgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as ChatDetailModel;
            if (messageVm == null)
                return null;
            return messageVm.IsIncoming ? this.outgoingDataTemplate : this.incomingDataTemplate;
        }

        private readonly DataTemplate incomingDataTemplate;
        private readonly DataTemplate outgoingDataTemplate;
    }
}
