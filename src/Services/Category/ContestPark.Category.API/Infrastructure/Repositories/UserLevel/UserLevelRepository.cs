using ContestPark.Core.Database.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.UserLevel
{
    public class UserLevelRepository : IUserLevelRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.UserLevel> _userLeveRepository;

        #endregion Private Variables

        #region Constructor

        public UserLevelRepository(IRepository<Tables.UserLevel> userLeveRepository)
        {
            _userLeveRepository = userLeveRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Oyuncunun seviyesine parametreden gelen exp değeri ile toplar eğer level atlamışsa leveli atlatır
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="exp">Kazanılan deneyim puanı</param>
        /// <returns>İşlem başarılı ise true başarısız ise false döner</returns>
        public Task<bool> UpdateLevel(string userId, short subCategoryId, byte exp)
        {
            return _userLeveRepository.ExecuteAsync("SP_UpdateLevel", new
            {
                userId,
                subCategoryId,
                exp
            }, CommandType.StoredProcedure);
        }

        /// <summary>
        /// Oyuncunun ilgili kategorideki levelini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>O kategorideki leveli</returns>
        public short GetUserLevel(string userId, short subCategoryId)
        {
            if (userId.EndsWith("-bot"))
                return 1;

            string sql = @"SELECT ul.Level
                           FROM UserLevels ul
                           WHERE ul.UserId = @userId
                           AND ul.SubCategoryId = @subCategoryId";
            short userLevel = _userLeveRepository.QuerySingleOrDefault<short>(sql, new
            {
                userId,
                subCategoryId
            });
            if (userLevel == 0)
            {
                _userLeveRepository.AddAsync(new Tables.UserLevel
                {
                    UserId = userId,
                    SubCategoryId = subCategoryId,
                    Level = 1,
                    Exp = 0,
                });

                userLevel = 1;
            }

            return userLevel;
        }

        #endregion Methods
    }
}
