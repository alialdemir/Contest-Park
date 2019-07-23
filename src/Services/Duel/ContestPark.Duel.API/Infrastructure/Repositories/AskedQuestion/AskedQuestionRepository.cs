using ContestPark.Core.Database.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.AskedQuestion
{
    public class AskedQuestionRepository : IAskedQuestionRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.AskedQuestion> _askedQuestionRepository;
        private readonly ILogger<AskedQuestionRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public AskedQuestionRepository(IRepository<Tables.AskedQuestion> askedQuestionRepository,
                                       ILogger<AskedQuestionRepository> logger)
        {
            _askedQuestionRepository = askedQuestionRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Sorulan soruları temizle
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="userIds">Kullanıcı idleri</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<bool> Delete(short subCategoryId, params string[] userIds)
        {
            string sql = "DELETE FROM AskedQuestions WHERE SubCategoryId = @subCategoryId AND UserId IN @userIds";

            return _askedQuestionRepository.ExecuteAsync(sql, new
            {
                subCategoryId,
                userIds
            });
        }

        public void Insert(short subCategoryId, int[] questions, params string[] userIds)
        {
            Task.Factory.StartNew(async () =>
            {
                List<Tables.AskedQuestion> askedQuestions = new List<Tables.AskedQuestion>();

                foreach (int item in questions)
                {
                    foreach (string userId in userIds)
                    {
                        askedQuestions.Add(new Tables.AskedQuestion
                        {
                            QuestionId = item,
                            SubCategoryId = subCategoryId,
                            UserId = userId
                        });
                    }
                }

                bool isSuccess = await _askedQuestionRepository.AddRangeAsync(askedQuestions);
                if (!isSuccess)
                {
                    _logger.LogCritical("CRITICAL: Sorulan sorular eklenirken hata oluştu.");
                }
            });
        }

        #endregion Methods
    }
}
