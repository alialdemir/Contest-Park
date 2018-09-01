namespace ContestPark.Domain.Question.Model.Response
{
    public class Answer
    {
        public byte Language { get; set; }

        public bool IsCorrect { get; set; }

        public string Answers { get; set; }
    }
}