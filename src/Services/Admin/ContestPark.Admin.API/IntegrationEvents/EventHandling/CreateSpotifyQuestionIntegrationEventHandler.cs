using ContestPark.Admin.API.Enums;
using ContestPark.Admin.API.IntegrationEvents.Events;
using ContestPark.Admin.API.Model.Spotify;
using ContestPark.Admin.API.Services.Spotify;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.IntegrationEvents.EventHandling
{
    public class CreateSpotifyQuestionIntegrationEventHandler : IIntegrationEventHandler<CreateSpotifyQuestionIntegrationEvent>
    {
        #region Private Variables

        private readonly IEventBus _eventBus;
        private readonly ISpotifyService _spotifyService;
        private readonly ILogger<CreateSpotifyQuestionIntegrationEventHandler> _logger;
        private readonly AdminSettings _adminSettings;

        #endregion Private Variables

        #region Constructor

        public CreateSpotifyQuestionIntegrationEventHandler(IEventBus eventBus,
                                                            ISpotifyService spotifyService,
                                                            ILogger<CreateSpotifyQuestionIntegrationEventHandler> logger,
                                                            IOptions<AdminSettings> options)
        {
            _eventBus = eventBus;
            _spotifyService = spotifyService;
            _logger = logger;
            _adminSettings = options.Value;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Spotify servisinden SpotifyQuestionTypes tipine göre soru bilgileri çeker
        /// </summary>
        /// <param name="spotifyQuestionType"></param>
        /// <param name="spotifyId"></param>
        /// <returns></returns>
        private Task<SpotifyModel> GetSpotifyModel(SpotifyQuestionTypes spotifyQuestionType, string spotifyId)
        {
            switch (spotifyQuestionType)
            {
                case Enums.SpotifyQuestionTypes.Playlist:
                    return _spotifyService.GetPlaylistQuestions(spotifyId);

                case Enums.SpotifyQuestionTypes.Artist:
                    return _spotifyService.GetArtistQuestionAsync(spotifyId);
            }

            return Task.FromResult(new SpotifyModel());
        }

        public async Task Handle(CreateSpotifyQuestionIntegrationEvent @event)
        {
            _logger.LogInformation("Spotify soru oluşturma işlemi başlıyor. SubCategoryId: {SubCategoryId} SpotifyId: {SpotifyId} SpotifyQuestionType: {SpotifyQuestionType}",
                                   @event.SubCategoryId,
                                   @event.SpotifyId,
                                   @event.SpotifyQuestionType);

            SpotifyModel spotifyModel = await GetSpotifyModel(@event.SpotifyQuestionType, @event.SpotifyId);
            if (!spotifyModel.Albums.Any() || !spotifyModel.Artists.Any() || !spotifyModel.Tracks.Any())
            {
                _logger.LogInformation("Spotify soru hazırlama bilgilerinin hepsi boş geldi. Spotify id {SpotifyId}", @event.SpotifyId);

                return;
            }

            SpotifyQuestionManager spotifyQuestionManager = new SpotifyQuestionManager(spotifyModel, @event.SubCategoryId, @event.SpotifyQuestionType);
            if (!spotifyQuestionManager.Questions.Any())
            {
                _logger.LogWarning("Spotift soru listesi boş geldi. Spotify id {SpotifyId}", @event.SpotifyId);

                return;
            }

            var @eventQuestion = new CreateQuestionIntegrationEvent(spotifyQuestionManager.Questions);

            _eventBus.Publish(@eventQuestion);
        }

        #endregion Methods
    }
}
