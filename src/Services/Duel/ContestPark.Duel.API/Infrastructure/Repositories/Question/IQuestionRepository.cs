namespace ContestPark.Duel.API.Infrastructure.Repositories.Question
{
    public interface IQuestionRepository
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Models.QuestionModel>> DuelQuestions(short subCategoryId, string founderUserId, string opponentUserId, Core.Enums.Languages founderLanguge, Core.Enums.Languages opponentLanguge);
    }
}
