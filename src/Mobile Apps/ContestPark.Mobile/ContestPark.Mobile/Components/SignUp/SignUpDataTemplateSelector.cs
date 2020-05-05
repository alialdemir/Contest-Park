using ContestPark.Mobile.Models.Login;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components.SignUp
{
    public class SignUpataTemplateSelector : DataTemplateSelector

    {
        public SignUpataTemplateSelector()
        {
            // Retain instances!
            this.inputDataTemplate = new DataTemplate(typeof(InputViewCell));
            this.categoriesDataTemplate = new DataTemplate(typeof(CategoriesViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (!(item is SignUpViewModel messageVm))
                return null;

            return messageVm.SignUpType == Enums.SignUpTypes.Categories ? this.categoriesDataTemplate : this.inputDataTemplate;
        }

        private readonly DataTemplate inputDataTemplate;
        private readonly DataTemplate categoriesDataTemplate;
    }
}
