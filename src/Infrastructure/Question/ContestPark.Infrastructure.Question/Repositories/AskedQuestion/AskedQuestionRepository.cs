using ContestPark.Core.Dapper;
using ContestPark.Core.Interfaces;
using ContestPark.Infrastructure.Question.Entities;
using Dapper;
using System;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Question.Repositories.AskedQuestion
{
    public class AskedQuestionRepository : DapperRepositoryBase<AskedQuestionEntity>, IAskedQuestionRepository
    {
        #region Constructor

        public AskedQuestionRepository(ISettingsBase settings) : base(settings)
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Parametreden kullanıcı idlerinin o kategorideki daha önceden sorulan sorularını siler
        /// </summary>
        /// <param name="founderUserId">Kurucu kullanıcı id</param>
        /// <param name="opponentUserId">Rakip kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns></returns>
        public async Task DeleteAsync(string founderUserId, string opponentUserId, Int16 subCategoryId)
        {
            string sql = @"DELETE FROM [AskedQuestions] WHERE UserId=@founderUserId and SubCategoryId=@subCategoryId;
                           DELETE FROM [AskedQuestions] WHERE UserId=@opponentUserId and SubCategoryId=@subCategoryId;";

            await Connection.ExecuteAsync(sql, new { founderUserId, opponentUserId, subCategoryId });
        }

        #endregion Methods
    }
}