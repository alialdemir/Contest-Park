using ContestPark.BackgroundTasks.IntegrationEvents.Events;
using ContestPark.BackgroundTasks.Models;
using ContestPark.BackgroundTasks.Services.Duel;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContestPark.BackgroundTasks.Tasks
{
    public class NewContestDateTask : BackgroundService
    {
        #region Private Variables

        private readonly ILogger<NewContestDateTask> _logger;
        private readonly IEventBus _eventBus;
        private readonly IDuelService _duelService;
        private Timer _timer;

        #endregion Private Variables

        #region Constructor

        public NewContestDateTask(ILogger<NewContestDateTask> logger,
                                  IEventBus eventBus,
                                  IDuelService duelService)
        {
            _logger = logger;
            _eventBus = eventBus;
            _duelService = duelService;
        }

        #endregion Constructor

        #region Methods

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Background task başlangıç
        /// </summary>
        /// <param name="stoppingToken"></param>
        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Yeni yarışma başlatma task başlatılıyor.");

            ContestDateModel contestDate = await _duelService.ActiveContestDate();
            if (contestDate == null)
            {
                _logger.LogWarning("Yarışma bitiş bilgileri boş geldi.");

                await Task.Delay(3000);

                await StartAsync(stoppingToken);

                return;
            }

            contestDate.ContestDateId = 3;
            contestDate.FinishDate = DateTime.Now.AddSeconds(60);

            _logger.LogInformation("{contestDateId} numaralı yarışma bitiş tarihi: {finishDate}", contestDate.ContestDateId, contestDate.FinishDate);

            TimeSpan diff = contestDate.FinishDate - DateTime.Now;

            _timer = new Timer(DeliverGoldToWinners,
                               contestDate.ContestDateId,
                               diff,
                               TimeSpan.FromMinutes(1));

            _logger.LogInformation("Yeni yarışma başlatma task başladı.");
        }

        /// <summary>
        /// Yarışmanın sona erdiğini event olarak publish eder
        /// </summary>
        /// <param name="contestDate"></param>
        private void DeliverGoldToWinners(object contestDate)
        {
            // TODO: Eğer yarışma tarihi bitmiş ise her alt kategoride aktif olan yarışmadaki ilk 3 kullanıcıyı getir
            // TODO: yeni tarih ekle
            // TODO: ilk üçe giren tüm kullanıcılara altın ver
            _timer?.Change(Timeout.Infinite, 0);

            var @event = new DeliverGoldToWinnersIntegrationEvent((short)contestDate);
            _eventBus.Publish(@event);

            _logger.LogInformation("Yarışma kazananlarına altınlar eklendi.");
        }

        /// <summary>
        /// Background task durdur
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Yeni yarışma başlatma task durdu.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _timer?.Dispose();

            _logger.LogInformation("Yeni yarışma başlatma timer dispose edildi.");
        }

        #endregion Methods
    }
}
