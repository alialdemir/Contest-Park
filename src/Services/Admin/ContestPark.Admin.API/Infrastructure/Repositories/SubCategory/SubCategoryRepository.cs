using ContestPark.Core.Database.Interfaces;

namespace ContestPark.Admin.API.Infrastructure.Repositories.SubCategory
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        #region Private Variables

        private readonly IQueryRepository queryRepository;

        #endregion Private Variables

        #region Constructor

        public SubCategoryRepository(IQueryRepository queryRepository)
        {
            this.queryRepository = queryRepository;
        }

        #endregion Constructor
    }
}
