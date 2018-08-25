using ContestPark.Domain.Question.Interfaces;
using ContestPark.Infrastructure.Question.Entities;
using ContestPark.Infrastructure.Question.Repositories.AskedQuestion;
using Orleans;
using System;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Question.Grains
{
    public class AskedQuestionGrain : Grain, IAskedQuestionGrain
    {
        #region Private variables

        private readonly IAskedQuestionRepository _askedQuestionRepository;

        #endregion Private variables

        #region Constructor

        public AskedQuestionGrain(IAskedQuestionRepository askedQuestionRepository)
        {
            _askedQuestionRepository = askedQuestionRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Çoklu sorulan sorulara ekleme
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="questionInfoId">Soru id</param>
        /// <param name="userIds">Kullanıcı idleri</param>
        public Task Insert(Int16 subCategoryId, int questionInfoId, params string[] userIds)
        {
            foreach (string userId in userIds)
            {
                if (userId.Contains("-bot"))
                    continue;

                _askedQuestionRepository
                    .Insert(new AskedQuestionEntity
                    {
                        SubCategoryId = subCategoryId,
                        QuestionInfoId = questionInfoId,
                        UserId = userId
                    });
            }
            return Task.CompletedTask;
        }

        #endregion Methods
    }
}