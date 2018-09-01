using ContestPark.Core.Dapper;
using ContestPark.Core.Domain.Model;
using ContestPark.Core.Extensions;
using ContestPark.Core.Interfaces;
using Dapper;

namespace ContestPark.Infrastructure.Identity.Repositories.User
{
    public class UserRepository : DatabaseConnection, IUserRepository
    {
        #region Constructor

        public UserRepository(ISettingsBase settings) : base(settings)
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Random kullanici resim listesi döndürür
        /// </summary>
        /// <param name="userId">Profil resmi istenmeyen kullanıcı id</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Profil resimleri service modeli</returns>
        public ServiceResponse<string> RandomUserProfilePictures(string userId, Paging pagingModel)
        {
            string sql = @"SELECT [AspNetUsers].[ProfilePicturePath]
                           FROM [AspNetUsers]
                           WHERE [AspNetUsers].[ProfilePicturePath] IS NOT NULL AND [AspNetUsers].[Id]<>@userId
                           ORDER BY NEWID()";

            return Connection.QueryPaging<string>(sql, new { userId, }, pagingModel);
        }

        /// <summary>
        /// Rastgele bot kullanıcılarından birini user id döner
        /// </summary>
        /// <returns>Rastlele bot user id</returns>
        public string GetRandomBotUserId()
        {
            string sql = @"SELECT [AspNetUsers].[Id]
                           FROM [AspNetUsers]
                           WHERE [AspNetUsers].[IsBot]=1
                           ORDER BY NEWID()";

            return Connection.QueryFirstOrDefault<string>(sql);
        }

        #endregion Methods
    }
}