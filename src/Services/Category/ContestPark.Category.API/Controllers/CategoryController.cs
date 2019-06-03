using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using ContestPark.Category.API.Infrastructure.Documents;

namespace ContestPark.Category.API.Controllers
{
    public class CategoryController : Core.Controllers.ControllerBase
    {
        #region Private variables

        private readonly ILogger<CategoryController> _logger;

        #endregion Private variables

        #region Constructor

        public CategoryController(ILogger<CategoryController> logger) : base(logger)
        {
            _logger = logger;
        }

        #endregion Constructor

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
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