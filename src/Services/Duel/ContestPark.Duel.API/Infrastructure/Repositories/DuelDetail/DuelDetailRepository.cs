using ContestPark.Core.Database.Interfaces;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail
{
    public class DuelDetailRepository : IDuelDetailRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.DuelDetail> _duelDetailrepository;
        private readonly ILogger<DuelDetailRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public DuelDetailRepository(IRepository<Tables.DuelDetail> duelDetailrepository,
                                    ILogger<DuelDetailRepository> logger)
        {
            _duelDetailrepository = duelDetailrepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Düello detayları ekler
        /// </summary>
        /// <param name="duelDetails">Düello bilgileri</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<bool> AddRangeAsync(IEnumerable<DuelDetailModel> duelDetails)
        {
            return _duelDetailrepository.AddRangeAsync(duelDetails.Select(x => new Tables.DuelDetail
            {
                DuelId = x.DuelId,
                QuestionId = x.QuestionId,
                CorrectAnswer = x.CorrectAnswer
            }).AsEnumerable());
        }

        /// <summary>
        /// Duello id ve question id'ye göre doğru cevap şıkkının yerini(A,B,C,D) verir
        /// </summary>
        /// <param name="duelId">Duello id</param>
        /// <param name="questionId">Soru id</param>
        /// <returns>Doğru cevap şıkkı</returns>
        public Stylish GetCorrectAnswer(int duelId, int questionId)
        {
            string sql = @"SELECT dd.CorrectAnswer
                           FROM DuelDetails dd
                           WHERE dd.DuelId = @duelId AND dd.QuestionId = @questionId";

            return _duelDetailrepository.QuerySingleOrDefault<Stylish>(sql, new
            {
                duelId,
                questionId
            });
        }

        /// <summary>
        /// Toplu olarak sorulara verilen cevapları günceller
        /// </summary>
        /// <param name="userAnswers">Sorulara verilen cevvap bilgileri</param>
        public Task UpdateDuelDetailAsync(List<UserAnswerModel> userAnswers)
        {
            string sql = CreateUpdateSqlScript(userAnswers);

            return _duelDetailrepository.ExecuteAsync(sql);
        }

        /// <summary>
        /// Duel details update scripti oluşturur
        /// </summary>
        /// <param name="userAnswers">Sorulara verilen cevaplar</param>
        /// <returns></returns>
        private string CreateUpdateSqlScript(List<UserAnswerModel> userAnswers)// TODO: performans için optimize edilmeli
        {
            string sql = "";

            foreach (UserAnswerModel userAnswer in userAnswers.GroupBy(x => x.QuestionId).ToList())
            {
                UserAnswerModel founderAnswer = userAnswers.FirstOrDefault(x => x.IsFounder && x.DuelId == userAnswer.DuelId && x.QuestionId == userAnswer.QuestionId);
                UserAnswerModel opponentAnswer = userAnswers.FirstOrDefault(x => !x.IsFounder && x.DuelId == userAnswer.DuelId && x.QuestionId == userAnswer.QuestionId);

                sql += $@"
UPDATE DuelDetails SET
FounderAnswer = {(byte)founderAnswer.Stylish},
OpponentAnswer = {(byte)opponentAnswer.Stylish},
FounderTime = {founderAnswer.Time},
OpponentTime = {opponentAnswer.Time},
FounderScore = {founderAnswer.Score},
OpponentScore = {opponentAnswer.Score},
ModifiedDate = NOW()
WHERE DuelId= {founderAnswer.DuelId} AND QuestionId = {founderAnswer.QuestionId};";
            }

            _logger.LogInformation("Duel detail güncelleme scripti", sql);

            return sql;
        }

        #endregion Methods
    }
}
