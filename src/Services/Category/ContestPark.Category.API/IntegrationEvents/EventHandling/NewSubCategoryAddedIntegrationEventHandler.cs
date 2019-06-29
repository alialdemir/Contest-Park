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
                foreach (var item in @event.SubCategoryLocalized)
                {
                    List<string> suggestInputs = new List<string>();

                    string categoryName = @event.CategoryLocalized.Where(x => x.Language == item.Language).FirstOrDefault().Text;
                    string[] nameSplit = item.Text.Split(" ");
                    if (nameSplit.Count() > 1)// Birden fazla kelimeden oluşuyorsa bütün olarakta ekledik
                        suggestInputs.Add(item.Text);

                    suggestInputs.AddRange(nameSplit);// alt kategorinin adı suggest olarak eklendi
                    suggestInputs.AddRange(categoryName.Split(" "));// category adı suggest olarak eklendi

                    _searchRepository.Insert(new Search
                    {
                        SearchType = SearchTypes.Category,
                        DisplayPrice = @event.DisplayPrice,
                        Id = @event.SubCategoryId,
                        Price = @event.Price,
                        PicturePath = @event.PicturePath,
                        SubCategoryId = @event.SubCategoryId,
                        CategoryId = @event.CategoryId,
                        SubCategoryName = item.Text,
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
                _logger.LogCritical($"Yeni kategori elasticsearch eklenme hatası. userId: {@event.SubCategoryId} event Id: {@event.Id}", ex);

                return Task.FromException(ex);
            }
        }
    }
}