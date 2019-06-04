using ContestPark.Core.CosmosDb.Models;

namespace ContestPark.Category.API.Infrastructure.Documents
{
    public class FollowSubCategory : DocumentBase
    {
        public string SubCategoryId { get; set; }
        public string UserId { get; set; }
    }
}