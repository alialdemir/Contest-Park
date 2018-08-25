namespace ContestPark.Mobile.Models.Duel.Quiz
{
    public class AnswerPair //: Tuple<AnswerModel, AnswerModel>
    {
        public AnswerModel Item1 { get; set; }

        public AnswerModel Item2 { get; set; }

        public AnswerModel Item3 { get; set; }

        public AnswerModel Item4 { get; set; }

        public AnswerPair(AnswerModel item1, AnswerModel item2, AnswerModel item3, AnswerModel item4)
        //: base(item1, item2)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }
    }
}