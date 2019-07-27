using ContestPark.Core.Database.Interfaces;
using ContestPark.Duel.API.Enums;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Duel
{
    public class DuelRepository : IDuelRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Duel> _duelRepository;

        #endregion Private Variables

        #region Constructor

        public DuelRepository(IRepository<Tables.Duel> duelRepository)
        {
            _duelRepository = duelRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Duello ekle
        /// </summary>
        /// <param name="duel">Duello bilgileri</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<int?> Insert(Tables.Duel duel)
        {
            return _duelRepository.AddAndGetIdAsync(duel);
        }

        /// <summary>
        /// Duello id'ye ait düelloyu verir
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <returns>Düello model</returns>
        public Tables.Duel GetDuelByDuelId(int duelId)
        {
            string sql = "SELECT * FROM Duels d WHERE d.DuelId = @duelId AND d.DuelType = @duelType";

            return _duelRepository.QuerySingleOrDefault<Tables.Duel>(sql, new
            {
                duelId,
                duelType = (byte)DuelTypes.Created
            });
        }

        /// <summary>
        /// Düello güncelle
        /// </summary>
        /// <param name="duel">Düello bilgileri</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<bool> UpdateAsync(Tables.Duel duel)
        {
            return _duelRepository.UpdateAsync(duel);
        }

        #endregion Methods
    }
}
