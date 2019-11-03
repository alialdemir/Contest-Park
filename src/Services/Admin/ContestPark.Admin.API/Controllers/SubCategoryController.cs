using ContestPark.Admin.API.Infrastructure.Repositories.SubCategory;
using ContestPark.Admin.API.Model.SubCategory;
using ContestPark.Core.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace ContestPark.Admin.API.Controllers
{
    public class SubCategoryController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly ISubCategoryRepository _subCategoryRepository;

        #endregion Private Variables

        #region Constructor

        public SubCategoryController(ILogger<SubCategoryController> logger,
                                     ISubCategoryRepository subCategoryRepository) : base(logger)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Alt kategori adını ve alt kategori id listesi döndürür
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modu bilgileri</param>
        [HttpGet("Dropdown")]
        [ProducesResponseType(typeof(ServiceModel<SubCategoryDropdownModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetSubCategories([FromQuery]PagingModel paging)// Oyunucunun karşısına rakip ekler
        {
            var subCategories = _subCategoryRepository.GetSubCategories(CurrentUserLanguage, paging);
            if (subCategories == null || subCategories.Items.Count() == 0)
                return NotFound();

            return Ok(subCategories);
        }

        #endregion Methods
    }
}
