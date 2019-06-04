using ContestPark.Category.API.Infrastructure.Repositories.Category;
using ContestPark.Core.CosmosDb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContestPark.Category.API.Controllers
{
    public class CategoryController : Core.Controllers.ControllerBase
    {
        #region Private variables

        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryRepository _categoryRepository;

        #endregion Private variables

        #region Constructor

        public CategoryController(ILogger<CategoryController> logger,
                                  ICategoryRepository categoryRepository) : base(logger)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        #endregion Constructor

        // GET api/values
        [HttpGet]
        public IActionResult Get([FromQuery]PagingModel paingModel)
        {
            return Ok(_categoryRepository.GetCategories(UserId, CurrentUserLanguage, paingModel));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}