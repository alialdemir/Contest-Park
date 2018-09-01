using ContestPark.Core.Domain.Model;
using ContestPark.Domain.Identity.Interfaces;
using ContestPark.Infrastructure.Identity.Repositories.User;
using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Identity.Grains
{
    public class UserGrain : Grain, IUserGrain
    {
        #region Private variables

        private readonly IUserRepository _userRepository;

        #endregion Private variables

        #region Constructor

        public UserGrain(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kategorilerin listesi
        /// </summary>
        /// <param name="userId">Kullanıcı Id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Kategori listesi</returns>
        public Task<ServiceResponse<string>> RandomUserProfilePictures(string userId, Paging paging)
        {
            if (string.IsNullOrEmpty(userId))
                return Task.FromResult(new ServiceResponse<string>());

            return Task.FromResult(_userRepository.RandomUserProfilePictures(userId, paging));
        }

        /// <summary>
        /// Rastgele bot kullanıcılarından birini user id döner
        /// </summary>
        /// <returns>Rastlele bot user id</returns>
        public Task<string> GetRandomBotUserId()
        {
            string randomUserId = _userRepository.GetRandomBotUserId();

            return Task.FromResult(randomUserId);
        }

        #endregion Methods
    }
}