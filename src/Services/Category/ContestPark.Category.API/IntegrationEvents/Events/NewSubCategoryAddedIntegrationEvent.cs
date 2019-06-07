using ContestPark.Category.API.Model;
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
        public List<LanguageModel> SubCategoryLangs { get; set; }
        public List<LanguageModel> CategoryLangs { get; set; }

        public NewSubCategoryAddedIntegrationEvent(string displayPrice,
                                                   string picturePath,
                                                   int price,
                                                   string subCategoryId,
                                                   string categoryId,
                                                   List<LanguageModel> subCategoryLangs,
                                                   List<LanguageModel> categoryLangs)
        {
            DisplayPrice = displayPrice;
            PicturePath = picturePath;
            Price = price;
            SubCategoryLangs = subCategoryLangs;
            CategoryLangs = categoryLangs;

            SubCategoryId = subCategoryId;
            CategoryId = categoryId;
        }
    }
}