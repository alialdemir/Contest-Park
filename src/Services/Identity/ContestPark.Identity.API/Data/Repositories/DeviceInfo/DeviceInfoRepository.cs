using System.Linq;

namespace ContestPark.Identity.API.Data.Repositories.DeviceInfo
{
    public class DeviceInfoRepository : IDeviceInfoRepository
    {
        #region Private variables

        private readonly ApplicationDbContext _applicationDbContext;

        #endregion Private variables

        #region Constructor

        public DeviceInfoRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// DeviceIdentifier ekleme işlemi yapar
        /// </summary>
        /// <param name="deviceIdentifier"></param>
        /// <returns></returns>
        public bool Insert(string userId, string deviceIdentifier)
        {
            _applicationDbContext.DeviceInfos.Add(new Tables.DeviceInfo
            {
                UserId = userId,
                DeviceIdentifier = deviceIdentifier
            });

            return _applicationDbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// DeviceIdentifier kayıtlı mı kontrol eder
        /// </summary>
        /// <param name="deviceIdentifier"></param>
        /// <returns></returns>
        public bool CheckDeviceIdentifier(string deviceIdentifier)
        {
            return _applicationDbContext
                                    .DeviceInfos
                                    .Count(x => x.DeviceIdentifier == deviceIdentifier) > 3;
        }

        #endregion Methods
    }
}
