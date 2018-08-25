﻿using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Services.Signalr.Base;
using System;

namespace ContestPark.Mobile.Services.Signalr.Duel
{
    public class DuelSignalRService : IDuelSignalRService
    {
        #region Private

        private readonly ISignalRServiceBase _signalRService;

        #endregion Private

        #region Constructor

        public DuelSignalRService(ISignalRServiceBase signalRService)
        {
            _signalRService = signalRService ?? throw new ArgumentNullException(nameof(signalRService));
        }

        #endregion Constructor

        #region Events

        public EventHandler<DuelStartingModel> DuelScreenInfoEventHandler { get; set; }

        public EventHandler<QuestionModel> NextQuestionEventHandler { get; set; }

        #endregion Events

        #region Method

        /// <summary>
        /// Duello bekleme ekranındaki kullanıcı bilgilerini getirir
        /// </summary>
        public void DuelScreenInfo()
        {
            _signalRService?.On<DuelStartingModel>("DuelScreen", (data) => DuelScreenInfoEventHandler?.Invoke(data, null));
        }

        /// <summary>
        /// Sıradaki soruyu almak için
        /// </summary>
        public void NextQuestion()
        {
            _signalRService?.On<QuestionModel>("NextQuestion", (data) => NextQuestionEventHandler?.Invoke(data, null));
        }

        /// <summary>
        /// On DuelScreen kısmını kapatır
        /// </summary>
        public void OffDuelScreenInfo()
        {
            _signalRService?.Off("DuelScreen");
        }

        /// <summary>
        /// On NextQuestion kısmını kapatır
        /// </summary>
        public void OffNextQuestion()
        {
            _signalRService?.Off("NextQuestion");
        }

        #endregion Method
    }
}