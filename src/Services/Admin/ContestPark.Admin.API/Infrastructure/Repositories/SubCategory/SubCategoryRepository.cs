using ContestPark.Admin.API.Model.SubCategory;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;

namespace ContestPark.Admin.API.Infrastructure.Repositories.SubCategory
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        #region Private Variables

        private readonly IQueryRepository _queryRepository;

        #endregion Private Variables

        #region Constructor

        public SubCategoryRepository(IQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Tüm alt kategorileri isim ve id listesi seçilen dile göre getirir
        /// </summary>
        /// <param name="language">Dil</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns></returns>
        public ServiceModel<SubCategoryDropdownModel> GetSubCategories(Languages language, PagingModel paging)
        {
            string sql = @"SELECT scl.SubCategoryName, scl.SubCategoryId
                           FROM SubCategoryLangs scl
                           WHERE scl.`Language`=@language";

            return _queryRepository.ToServiceModel<SubCategoryDropdownModel>(sql, new
            {
                language
            }, pagingModel: paging);
        }

        #endregion Methods
    }
}
