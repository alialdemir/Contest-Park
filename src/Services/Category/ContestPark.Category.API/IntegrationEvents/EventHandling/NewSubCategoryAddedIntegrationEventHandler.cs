using ContestPark.Category.API.Infrastructure.Documents;
using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Category.API.IntegrationEvents.EventHandling
{
    public class NewSubCategoryAddedIntegrationEventHandler : IIntegrationEventHandler<NewSubCategoryAddedIntegrationEvent>

    {
        private readonly ISearchRepository _searchRepository;
        private readonly ILogger<NewSubCategoryAddedIntegrationEventHandler> _logger;

        public NewSubCategoryAddedIntegrationEventHandler(ISearchRepository searchRepository,
                                                          ILogger<NewSubCategoryAddedIntegrationEventHandler> logger)
        {
            _searchRepository = searchRepository;
            _logger = logger;
        }

        /// <summary>
        /// Yeni alt kategori eklenince elasticsearch e o alt kategori bilgilerini ekler
        /// </summary>
        public Task Handle(NewSubCategoryAddedIntegrationEvent @event)
        {
            try
            {
                foreach (var item in @event.SubCategoryLangs)
                {
                    List<string> suggestInputs = new List<string>();

                    string categoryName = @event.CategoryLangs.Where(x => x.Language == item.Language).FirstOrDefault().Name;

                    suggestInputs.AddRange(item.Name.Split(" "));// alt kategorinin adı suggest olarak eklendi
                    suggestInputs.AddRange(categoryName.Split(" "));// category adı suggest olarak eklendi

                    _searchRepository.Insert(new Search
                    {
                        SearchType = Model.SearchTypes.Category,
                        DisplayPrice = @event.DisplayPrice,
                        Id = @event.SubCategoryId,
                        Price = @event.Price,
                        PicturePath = @event.PicturePath,
                        SubCategoryId = @event.SubCategoryId,
                        SubCategoryName = item.Name,
                        CategoryName = categoryName,
                        Language = item.Language,
                        Suggest = new Nest.CompletionField
                        {
                            Input = suggestInputs
                        },
                    });
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Yeni kategori elasticsearch eklenme hatası. userId: {@event.SubCategoryId} event Id: {@event.Id}");

                return Task.FromException(ex);
            }
        }
    }
}