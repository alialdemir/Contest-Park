using ContestPark.Admin.API.Enums;

namespace ContestPark.Admin.API.Model.Spotify
{
    public class MusicQuestionModel
    {
        public string QuestionTr { get; internal set; }
        public string QuestionEn { get; internal set; }
        public QuestionTypes QuestionType { get; internal set; }
    }
}
