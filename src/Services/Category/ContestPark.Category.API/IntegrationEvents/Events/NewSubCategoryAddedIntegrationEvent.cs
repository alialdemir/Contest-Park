using ContestPark.Core.Models;
using ContestPark.EventBus.Events;
using System.Collections.Generic;

namespace ContestPark.Category.API.IntegrationEvents.Events
{
    public class NewSubCategoryAddedIntegrationEvent : IntegrationEvent
    {
        public string DisplayPrice { get; set; }

        public string PicturePath { get; set; }

        public decimal Price { get; set; }

        public short SubCategoryId { get; set; }
        public short CategoryId { get; set; }
        public List<Localized> SubCategoryLocalized { get; set; }
        public List<Localized> CategoryLocalized { get; set; }

        public NewSubCategoryAddedIntegrationEvent(string displayPrice,
                                                   string picturePath,
                                                   decimal price,
                                                   short subCategoryId,
                                                   short categoryId,
                                                   List<Localized> subCategoryLocalized,
                                                   List<Localized> categoryLocalized)
        {
            DisplayPrice = displayPrice;
            PicturePath = picturePath;
            Price = price;
            SubCategoryLocalized = subCategoryLocalized;
            CategoryLocalized = categoryLocalized;

            SubCategoryId = subCategoryId;
            CategoryId = categoryId;
        }
    }
}
