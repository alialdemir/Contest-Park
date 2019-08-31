namespace ContestPark.Identity.API.Data.Repositories.ReferenceCode
{
    public class ReferenceCodeRepostory : IReferenceCodeRepostory
    {
        #region Private variables

        private readonly ApplicationDbContext _applicationDbContext;

        #endregion Private variables

        #region Constructor

        public ReferenceCodeRepostory(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Referans kodu ile yapılan işlemi ekle
        /// </summary>
        /// <param name="referenceCode">Referans kod</param>
        public void Insert(string code, string referenceUserId, string newUserId)
        {
            _applicationDbContext.ReferenceCodes.Add(new Tables.ReferenceCode
            {
                NewUserId = newUserId,
                ReferenceUserId = referenceUserId,
                Code = code
            });

            _applicationDbContext.SaveChanges();
        }

        #endregion Methods
    }
}
