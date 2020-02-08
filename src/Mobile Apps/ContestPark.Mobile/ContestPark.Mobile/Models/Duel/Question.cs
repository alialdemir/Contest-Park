using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Duel.Quiz;
using System.Collections.Generic;

namespace ContestPark.Mobile.Models.Duel
{
    public class Question : BaseModel
    {
        public int QuestionId { get; set; }
        private string _link;

        public string Link
        {
            get { return _link; }
            set
            {
                _link = value;
                RaisePropertyChanged(() => Link);
            }
        }

        public AnswerTypes AnswerType { get; set; }

        private QuestionTypes _questionType;

        public QuestionTypes QuestionType
        {
            get { return _questionType; }
            set
            {
                _questionType = value;
                RaisePropertyChanged(() => QuestionType);
            }
        }

        public List<AnswerModel> Answers { get; set; } = new List<AnswerModel>();

        public List<QuestionLocalizedModel> Questions { get; set; } = new List<QuestionLocalizedModel>();

        private string _nextQuestion;

        public string NextQuestion
        {
            get { return _nextQuestion; }
            set
            {
                _nextQuestion = value;
                RaisePropertyChanged(() => NextQuestion);
            }
        }
    }
}
