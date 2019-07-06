using ContestPark.Chat.API.Infrastructure.Repositories.Block;
using ContestPark.Chat.API.Model;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Mysql
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
            return _blockRepository.RemoveAsync(x => x.DeterredUserId == deterredUserId && x.SkirterUserId == skirterUserId);
        }

        /// <summary>
        /// Sohbet karşılıklı engelleme durumu
        /// </summary>
        /// <param name="skirterUserId">Engelleyen kullanıcı id</param>
        /// <param name="deterredUserId">Engellenen kullanıcı id</param>
        /// <returns>İki tarafdan biri engellemiş mi true değilse false</returns>
        public bool BlockingStatus(string skirterUserId, string deterredUserId)
        {
            string sql = @"SELECT TOP 1 VALUE true FROM c
                           WHERE (c.SkirterUserId=@skirterUserId AND c.DeterredUserId=@deterredUserId)
                              OR (c.SkirterUserId=@deterredUserId AND c.DeterredUserId=@skirterUserId)";

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
            string sql = @"SELECT VALUE {
                           IsBlocked: true,
                           UserId: c.DeterredUserId
                           } FROM c  WHERE c.SkirterUserId=@SkirterUserId
                           ORDER BY c.CreatedDate DESC";

            return _blockRepository.ToServiceModel<BlockModel>(sql, new
            {
                SkirterUserId,
            }, pagingModel: paging);
        }

        #endregion Methods
    }
}
