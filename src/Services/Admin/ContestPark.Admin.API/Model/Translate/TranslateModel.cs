using System.Collections.Generic;

namespace ContestPark.Admin.API.Model.Translate
{
    public class TranslateModel
    {
        public TranslateDataModel Data { get; set; }
    }

    public class TranslateDataModel
    {
        public List<TranslationsModel> Translations { get; set; }
    }

    public class TranslationsModel
    {
        public string TranslatedText { get; set; }
    }
}
