using ContestPark.Core.Models;
using ContestPark.EventBus.Events;
using System.Collections.Generic;

namespace ContestPark.Category.API.IntegrationEvents.Events
{
    public class NewSubCategoryAddedIntegrationEvent : IntegrationEvent
    {
        public string DisplayPrice { get; set; }

        public string PicturePath { get; set; }

        public int Price { get; set; }

        public string SubCategoryId { get; set; }
        public string CategoryId { get; set; }
        public List<Localized> SubCategoryLocalized { get; set; }
        public List<Localized> CategoryLocalized { get; set; }

        public NewSubCategoryAddedIntegrationEvent(string displayPrice,
                                                   string picturePath,
                                                   int price,
                                                   string subCategoryId,
                                                   string categoryId,
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