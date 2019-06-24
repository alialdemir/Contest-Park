using ContestPark.Core.CosmosDb.Interfaces;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Block
{
    public class BlockRepository : IBlockRepository
    {
        #region Private Variables

        private readonly IDocumentDbRepository<Documents.Block> _blockRepository;

        #endregion Private Variables

        #region Constructor

        public BlockRepository(IDocumentDbRepository<Documents.Block> blockRepository)
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

            return _blockRepository.AddAsync(new Documents.Block
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

            return _blockRepository.QuerySingle<bool>(sql, new
            {
                skirterUserId,
                deterredUserId
            });
        }

        #endregion Methods
    }
}