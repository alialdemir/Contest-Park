﻿using ContestPark.Admin.API.Infrastructure.Repositories.SubCategory;
using ContestPark.Admin.API.Model.SubCategory;
using ContestPark.Core.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
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
        /// Kategori güncelle
        /// </summary>
        /// <param name="subCategoryUpdate">Kategori bilgisi</param>
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateSubCategoryAsync([FromBody]SubCategoryUpdateModel subCategoryUpdate)
        {
            // TODO: Elasticsearch evemt publish
            // TODO: alt kategori resmini s3 kayıt edilecek
            bool isSuccess = await _subCategoryRepository.UpdateAsync(subCategoryUpdate);
            if (!isSuccess)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Alt kategori ekle
        /// </summary>
        /// <param name="subCategoryInsert">Alt kategori bilgisi</param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> InsertSubCategoryAsync([FromBody]SubCategoryInsertModel subCategoryInsert)
        {
            // TODO: Elasticsearch evemt publish
            // TODO: alt kategori resmini s3 kayıt edilecek
            bool isSuccess = await _subCategoryRepository.InsertAsync(subCategoryInsert);
            if (!isSuccess)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Alt kategori adını ve alt kategori id listesi döndürür
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modu bilgileri</param>
        [HttpGet("Dropdown")]
        [ProducesResponseType(typeof(ServiceModel<SubCategoryDropdownModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetSubCategories([FromQuery]PagingModel paging)
        {
            var subCategories = _subCategoryRepository.GetSubCategories(CurrentUserLanguage, paging);
            if (subCategories == null || subCategories.Items.Count() == 0)
                return NotFound();

            return Ok(subCategories);
        }

        #endregion Methods
    }
}
