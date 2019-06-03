using ContestPark.Core.CosmosDb.Models;

namespace ContestPark.Category.API.Infrastructure.Documents
{
    public class OpenSubCategory : DocumentBase
    {
        public string UserId { get; set; }
        public string SubCategoryId { get; set; }
    }
}