using ContestPark.Core.Dapper;
using ContestPark.Infrastructure.Question.Entities;
using System;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Question.Repositories.AskedQuestion
{
    public interface IAskedQuestionRepository : IRepository<AskedQuestionEntity>
    {
        Task DeleteAsync(string founderUserId, string opponentUserId, Int16 subCategoryId);
    }
}