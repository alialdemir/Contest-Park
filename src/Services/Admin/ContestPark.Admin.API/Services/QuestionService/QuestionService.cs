using ContestPark.Admin.API.Enums;
using ContestPark.Admin.API.Model.Question;
using ContestPark.Admin.API.Model.Translate;
using ContestPark.Core.Enums;
using ContestPark.Core.Services.RequestProvider;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Services.QuestionService
{
    public class QuestionService : IQuestionService
    {
        #region Private Variables

        private readonly IRequestProvider _requestProvider;
        private readonly ILogger<QuestionService> _logger;
        private readonly Dictionary<string, string> _translates = new Dictionary<string, string>();
        private readonly Regex _keyRegex = new Regex(@"[^{{\\}]+(?=}})");
        private const string _trCode = "tr";
        private const string _enCode = "en";

        #endregion Private Variables

        #region Constructor

        public QuestionService(IRequestProvider requestProvider,
                               ILogger<QuestionService> logger)
        {
            _requestProvider = requestProvider;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// IFormFile json dosyasını alır string olarak döndürür
        /// </summary>
        /// <param name="file">Json file</param>
        /// <returns>Json string</returns>
        private async Task<string> ConvertFileToJsonArraay(IFormFile file)
        {
            StreamReader reader = new StreamReader(file.OpenReadStream());
            string jsonQuestion = await reader.ReadToEndAsync();

            return jsonQuestion;
        }

        /// <summary>
        /// Json sorularını bizim database modeline göre hazırlar
        /// </summary>
        /// <param name="configModel">Soru hazırlama ayarları ve soru json</param>
        /// <returns>Sorular</returns>
        public async Task<List<QuestionSaveModel>> GenerateQuestion(QuestionConfigModel configModel)
        {
            if (configModel == null
                || configModel.SubCategoryId <= 0
                || configModel.File == null
                || configModel.File.Length == 0
                || string.IsNullOrEmpty(configModel.Question)
                || string.IsNullOrEmpty(configModel.AnswerKey))
                return null;

            _logger.LogInformation("Soru oluşturma servisi çağrıldı");

            string jsonQuestion = await ConvertFileToJsonArraay(configModel.File);
            if (string.IsNullOrEmpty(jsonQuestion))
                return null;

            JArray jsonArray = JArray.Parse(jsonQuestion);

            IEnumerable<KeyValuePair<string, string>> questions = JsonKeyValue(jsonQuestion);

            _logger.LogInformation("Json convert success");

            var answers = questions
                                .Where(x => x.Key == configModel.AnswerKey)
                                .GroupBy(x => x)
                                .Select(x => x.Key)
                                .Select(x => x.Value)
                                .ToList();

            bool isQuestionTurkishTranslate = configModel.QuestionLanguage == ConvertLanguageTypes.TurkishToEnglish;
            bool isAnswerTurkishTranslate = configModel.AnswerLanguage == ConvertLanguageTypes.TurkishToEnglish;

            Languages AnswerLanguage = configModel.AnswerLanguage == ConvertLanguageTypes.TurkishToEnglish
                ? Languages.Turkish
                : Languages.English;

            return (from item in jsonArray
                    let correctStylish = item[configModel.AnswerKey].ToString()
                    let question = GetQuestion(configModel.Question, item)
                    let answerSaveModel = GetRandomAnswersFromArray(answers, correctStylish, AnswerLanguage)
                    where !string.IsNullOrEmpty(correctStylish) && !string.IsNullOrEmpty(question)
                    select new QuestionSaveModel
                    {
                        Questions = new List<Question>
                    {
                        new Question
                        {
                            AnswerTypes= configModel.AnswerType,
                            QuestionType = configModel.QuestionType,
                            Link = !string.IsNullOrEmpty(configModel.LinkKey) && item.SelectToken(configModel.LinkKey).HasValues
                            ? item[configModel.LinkKey].ToString()
                            : string.Empty,
                            SubCategoryId = configModel.SubCategoryId,
                        }
                    },
                        QuestionLocalized = new List<QuestionLocalized>
                                {
                                    new QuestionLocalized
                                    {
                                        Language = isQuestionTurkishTranslate ? Languages.Turkish :  Languages.English,
                                        Question = question,
                                    },
                                    new QuestionLocalized
                                    {
                                        Language = isQuestionTurkishTranslate ? Languages.English :  Languages.Turkish,
                                        Question =Translate(
                                            isQuestionTurkishTranslate ? _enCode : _trCode,
                                            isQuestionTurkishTranslate ? _trCode : _enCode,
                                            question),
                                    }
                                },
                        Answers = new List<AnswerSaveModel>
                    {
                        answerSaveModel,
                        new AnswerSaveModel
                        {
                            Language = isAnswerTurkishTranslate ? Languages.English: Languages.Turkish,
                            CorrectStylish = UppercaseFirstLetter(Translate(
                                            isAnswerTurkishTranslate ? _enCode : _trCode,
                                            isAnswerTurkishTranslate ? _trCode : _enCode,
                                            answerSaveModel.CorrectStylish)),
                            Stylish1 = UppercaseFirstLetter(Translate(
                                            isAnswerTurkishTranslate ? _enCode : _trCode,
                                            isAnswerTurkishTranslate ? _trCode : _enCode,
                                            answerSaveModel.Stylish1)),
                            Stylish2 = UppercaseFirstLetter(Translate(
                                            isAnswerTurkishTranslate ? _enCode : _trCode,
                                            isAnswerTurkishTranslate ? _trCode : _enCode,
                                            answerSaveModel.Stylish2)),
                            Stylish3 = UppercaseFirstLetter(Translate(
                                            isAnswerTurkishTranslate ? _enCode : _trCode,
                                            isAnswerTurkishTranslate ? _trCode : _enCode,
                                            answerSaveModel.Stylish3)),
                        }
                    },
                    }).ToList();
        }

        /// <summary>
        /// Eğer soru içerisinde {{jsonKey}} varsa o kısmı json değeri ile değiştirir
        /// Örneğin
        /// json objesi { country:'Türkiye' } olsun
        /// Soru kalıbıda : {{country}} ülkesi nerededir?
        /// {{country}} gördüğü yere value ekler yani
        /// Türkiye ülkesi nerededir?
        /// olarak return eder eğer {{jsonKey}} yoksa gelen soruyu değiştirmeden geri döndürür
        /// </summary>
        /// <param name="question">Soru</param>
        /// <param name="token">Josn object</param>
        /// <returns>Yeni soru</returns>
        private string GetQuestion(string question, JToken token)
        {
            if (!_keyRegex.IsMatch(question))
                return question;

            string key = _keyRegex.Match(question).Value.ToLower();

            return question.Replace("{{" + key + "}}", token[key].ToString());
        }

        /// <summary>
        /// Jsonu key value şeklinde döndürür
        /// </summary>
        /// <param name="json">Json string</param>
        /// <returns>Json key value</returns>
        private IEnumerable<KeyValuePair<string, string>> JsonKeyValue(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            return JArray.Parse(json)
                 .Descendants()
                 .OfType<JProperty>()
                 .Select(p => new KeyValuePair<string, string>(p.Name,
                     p.Value.Type == JTokenType.Array || p.Value.Type == JTokenType.Object
                         ? "" : p.Value.ToString()));
        }

        private AnswerSaveModel GetRandomAnswersFromArray(List<string> answers, string correctStylish, Languages language)
        {
            if (answers == null || !answers.Any() || string.IsNullOrEmpty(correctStylish))
                return null;

            answers = answers
                       .Where(x => x != correctStylish && !string.IsNullOrEmpty(x))
                       .OrderBy(x => Guid.NewGuid())
                       .Take(3)
                       .ToList();

            if (answers.GroupBy(x => x).Count() != 3)
                return GetRandomAnswersFromArray(answers, correctStylish, language);

            return new AnswerSaveModel
            {
                Language = language,
                CorrectStylish = UppercaseFirstLetter(correctStylish),
                Stylish1 = UppercaseFirstLetter(answers[0]),
                Stylish2 = UppercaseFirstLetter(answers[1]),
                Stylish3 = UppercaseFirstLetter(answers[2])
            };
        }

        /// <summary>
        /// Kelimelerin baş harflerini büyük yapar
        /// </summary>
        /// <param name="title">Kelime</param>
        /// <returns>Baş harrfleri büyük olarak döndürür</returns>
        private string UppercaseFirstLetter(string title)
        {
            if (string.IsNullOrEmpty(title))
                return string.Empty;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower()).Trim();
        }

        /// <summary>
        /// Gelen text değerini google translate üzerinden çevirir
        /// </summary>
        /// <param name="target">Çevilecek dil kodu</param>
        /// <param name="source">Şuanki dil kodu</param>
        /// <param name="text">Çevrilecek yazı</param>
        /// <returns>Translate edilmiş yazı</returns>
        private string Translate(string target, string source, string text)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(source) || string.IsNullOrEmpty(text))
                return string.Empty;

            if (_translates.ContainsKey(text))
                return _translates.GetValueOrDefault(text);

            string url = $"https://www.googleapis.com/language/translate/v2?key=AIzaSyAeXkVGJsu5rn2U7gZLDk8qR7b0Zj23q6I&target={target}&source={source}&q={text}";
            var result = _requestProvider.GetAsync<TranslateModel>(url).Result;

            if (result == null || result.Data == null || !result.Data.Translations.Any())
                return string.Empty;

            string translateText = result.Data.Translations.LastOrDefault().TranslatedText;

            _translates.Add(text, translateText);

            return translateText;
        }

        #endregion Methods
    }
}
