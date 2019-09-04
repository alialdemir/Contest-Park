using ContestPark.Identity.API.Enums;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Data.Repositories.Picture
{
    public class PictureRepository : IPictureRepository
    {
        #region Private variables

        private readonly ApplicationDbContext _applicationDbContext;

        #endregion Private variables

        #region Constructor

        public PictureRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Resim ekle
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="pictureUrl">Resim url</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public bool Insert(string userId, string pictureUrl, PictureTypes pictureType)
        {
            _applicationDbContext.Pictures.Add(new Tables.Picture
            {
                UserId = userId,
                PictureUrl = pictureUrl,
                PictureType = pictureType
            });

            return _applicationDbContext.SaveChanges() > 0;
        }

        #endregion Methods
    }
}
