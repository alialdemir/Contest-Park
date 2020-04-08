using ContestPark.EventBus.Events;
using System.Collections.Generic;

namespace ContestPark.Category.API.IntegrationEvents.Events
{
    public class OpenSubCategoryAndFollowIntegrationEvent : IntegrationEvent
    {
        public OpenSubCategoryAndFollowIntegrationEvent(string userId,
                                                        IEnumerable<short> subCategories)
        {
            UserId = userId;
            SubCategories = subCategories;
        }

        public string UserId { get; set; }
        public IEnumerable<short> SubCategories { get; set; }
    }
}
