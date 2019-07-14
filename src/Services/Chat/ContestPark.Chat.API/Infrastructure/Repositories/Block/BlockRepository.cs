using ContestPark.Chat.API.Model;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Block
{
    public class BlockRepository : IBlockRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Block> _blockRepository;

        #endregion Private Variables

        #region Constructor

        public BlockRepository(IRepository<Tables.Block> blockRepository)
        {
            _blockRepository = blockRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı engelle
        /// </summary>
        /// <param name="skirterUserId">Engelleyen kullanıcı id</param>
        /// <param name="deterredUserId">Engellenen kullanıcı id</param>
        /// <returns>İşlem durumu başarılı ise true değilse false</returns>
        public Task<bool> BlockedAsync(string skirterUserId, string deterredUserId)
        {
            if (string.IsNullOrEmpty(skirterUserId) || string.IsNullOrEmpty(deterredUserId))
                return Task.FromResult(false);

            return _blockRepository.AddAsync(new Tables.Block
            {
                SkirterUserId = skirterUserId,
                DeterredUserId = deterredUserId
            });
        }

        /// <summary>
        /// Kullanıcı engellini kaldırır
        /// </summary>
        /// <param name="skirterUserId">Engelleyen kullanıcı id</param>
        /// <param name="deterredUserId">Engellenen kullanıcı id</param>
        /// <returns>İşlem durumu başarılı ise true değilse false</returns>
        public Task<bool> UnBlockAsync(string skirterUserId, string deterredUserId)
        {
            string sql = "DELETE FROM Blocks WHERE DeterredUserId = @deterredUserId AND SkirterUserId = @skirterUserId";

            return _blockRepository.ExecuteAsync(sql, new
            {
                skirterUserId,
                deterredUserId
            });
        }

        /// <summary>
        /// Sohbet karşılıklı engelleme durumu
        /// </summary>
        /// <param name="skirterUserId">Engelleyen kullanıcı id</param>
        /// <param name="deterredUserId">Engellenen kullanıcı id</param>
        /// <returns>İki tarafdan biri engellemiş mi true değilse false</returns>
        public bool BlockingStatus(string skirterUserId, string deterredUserId)
        {
            string sql = @"SELECT (CASE
                           WHEN EXISTS(
                           SELECT NULL
                           FROM Blocks b WHERE (b.SkirterUserId=@skirterUserId AND b.DeterredUserId=@deterredUserId)
                              OR (b.SkirterUserId=@deterredUserId AND b.DeterredUserId=@skirterUserId))
                           THEN 1
                           ELSE 0
                           END)";

            return _blockRepository.QuerySingleOrDefault<bool>(sql, new
            {
                skirterUserId,
                deterredUserId
            });
        }

        /// <summary>
        /// Engellediği kullanıcların listesi
        /// </summary>
        /// <param name="SkirterUserId">Engelleyen kullanıcı id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Engellenen kullanıcılar</returns>
        public ServiceModel<BlockModel> UserBlockedList(string SkirterUserId, PagingModel paging)
        {
            string sql = @"SELECT b.DeterredUserId AS UserId FROM Blocks b
                           WHERE b.SkirterUserId=@SkirterUserId
                           ORDER BY b.CreatedDate DESC";

            return _blockRepository.ToServiceModel<BlockModel>(sql, new
            {
                SkirterUserId,
            }, pagingModel: paging);
        }

        #endregion Methods
    }
}
