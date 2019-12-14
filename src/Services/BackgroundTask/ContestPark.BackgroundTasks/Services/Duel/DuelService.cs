using ContestPark.BackgroundTasks.Models;
using ContestPark.Core.Services.RequestProvider;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ContestPark.BackgroundTasks.Services.Duel
{
    public class DuelService : IDuelService
    {
        #region Private Variables

        private readonly IRequestProvider _requestProvider;

        private readonly string baseUrl = "";

        #endregion Private Variables

        #region Constructor

        public DuelService(IRequestProvider requestProvider,
                           IOptions<BackgroundTaskSettings> options)
        {
            _requestProvider = requestProvider ?? throw new ArgumentNullException(nameof(requestProvider));

            string identityUrl = options.Value.DuelUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            baseUrl = $"{identityUrl}/api/v1/Ranking";
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Aktif olan yarışma başlangıç ve bitiş tarihlerini verir
        /// </summary>
        /// <returns>Aktif olan yarışma tarihi</returns>
        public async Task<ContestDateModel> ActiveContestDate()
        {
            ContestDateModel contestDate = await _requestProvider.GetAsync<ContestDateModel>($"{baseUrl}/ActiveContestDate");

            return contestDate;
        }

        #endregion Methods
    }
}
