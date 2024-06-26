﻿using ContestPark.Core.Models;
using ContestPark.Core.Services.Identity;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.Duel.API.Resources;
using ContestPark.Duel.API.Services.Balance;
using ContestPark.Duel.API.Services.SubCategory;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Controllers
{
    public class DuelController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IEventBus _eventBus;
        private readonly IDuelRepository _duelRepository;
        private readonly IIdentityService _identityService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IBalanceService _balanceService;

        #endregion Private Variables

        #region Constructor

        public DuelController(ILogger<DuelController> logger,
                              IEventBus eventBus,
                              IIdentityService identityService,
                              ISubCategoryService subCategoryService,
                              IBalanceService balanceService,
                              IDuelRepository duelRepository) : base(logger)
        {
            _eventBus = eventBus;
            _identityService = identityService;
            _subCategoryService = subCategoryService;
            _balanceService = balanceService;
            _duelRepository = duelRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Düelloya davet et
        /// </summary>
        /// <param name="inviteDuel">Daveti bilgisi</param>
        /// <returns>Davet edilen kullanıcı bilgileri</returns>
        [HttpPost("Invite")]
        [ProducesResponseType(typeof(UserModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> InviteDuel([FromBody] InviteDuelModel inviteDuel)
        {
            Logger.LogInformation("Düello davet isteği geldi.");

            if (inviteDuel == null
                || string.IsNullOrEmpty(inviteDuel.OpponentUserId)
                || inviteDuel.SubCategoryId <= 0
                || inviteDuel.Bet < 0
                || UserId == inviteDuel.OpponentUserId)
                return BadRequest();

            Logger.LogInformation("Düello daveti gönderiliyor. {founderUserId} - {opponentUserId}", UserId, inviteDuel.OpponentUserId);

            #region Para ile düelloya davet iptal

            if (inviteDuel.BalanceType == Enums.BalanceTypes.Money && !(UserId.EndsWith("-bot") || inviteDuel.OpponentUserId.EndsWith("-bot")))
            {
                return BadRequest();
            }

            #endregion Para ile düelloya davet iptal

            BalanceModel balance = await _balanceService.GetBalance(UserId, inviteDuel.BalanceType);
            if (balance == null || inviteDuel.Bet > balance.Amount)
            {
                return BadRequest(DuelResource.YourBalanceIsInsufficient);
            }

            var opponentUserInfo = await _identityService.GetUserInfosAsync(new string[] { inviteDuel.OpponentUserId, UserId }, includeCoverPicturePath: true);
            if (opponentUserInfo == null || opponentUserInfo.Count() != 2)
                return BadRequest();

            UserModel opponentUserModel = opponentUserInfo.FirstOrDefault(x => x.UserId == inviteDuel.OpponentUserId);
            UserModel founderUserModel = opponentUserInfo.FirstOrDefault(x => x.UserId == UserId);

            #region Rakibin levelini alır

            IEnumerable<UserLevelModel> userLevels = await _subCategoryService.UserLevel(new List<string>
            {
                inviteDuel.OpponentUserId,
            }, inviteDuel.SubCategoryId);
            if (userLevels != null && userLevels.Any())
            {
                opponentUserModel.Level = userLevels.FirstOrDefault().Level;
            }

            #endregion Rakibin levelini alır

            #region Eğer davet ettiği oyuncu bot ise kabul etmiş sayıyoruz

            if (inviteDuel.OpponentUserId.EndsWith("-bot"))
            {
                Task.Factory.StartNew(async () =>
                {
                    Logger.LogInformation("Bot kullanıcısına düellod daveti geldi.", UserId, inviteDuel.OpponentUserId);

                    if (_duelRepository.PlaysInTheLastHour(UserId, inviteDuel.OpponentUserId))
                    {
                        return;
                    }

                    var eventDuelStart = new DuelStartIntegrationEvent(subCategoryId: inviteDuel.SubCategoryId,
                                                                       bet: inviteDuel.Bet,
                                                                       balanceType: inviteDuel.BalanceType,
                                                                       founderUserId: UserId,
                                                                       founderConnectionId: inviteDuel.FounderConnectionId,
                                                                       founderLanguage: CurrentUserLanguage,
                                                                       opponentUserId: inviteDuel.OpponentUserId,
                                                                       opponentConnectionId: "rnsadjge4",// fake signalr id
                                                                       opponentLanguage: CurrentUserLanguage);
                    await Task.Delay(new Random().Next(1, 3) * 1000);

                    _eventBus.Publish(eventDuelStart);

                    Logger.LogInformation("Bot kullanıcısına düellod daveti kabul edildi.", UserId, inviteDuel.OpponentUserId);
                }).Wait();

                return Ok(opponentUserModel);
            }

            #endregion Eğer davet ettiği oyuncu bot ise kabul etmiş sayıyoruz

            SubCategoryModel subCategory = await _subCategoryService.GetSubCategoryInfo(inviteDuel.SubCategoryId, CurrentUserLanguage, inviteDuel.OpponentUserId);// TODO: CurrentUserLanguage değeri rakibin dil seçeneği olmalı !
            if (subCategory == null)
                return BadRequest();

            var @event = new InviteDuelIntegrationEvent(opponentUserId: opponentUserModel.UserId,

                                                        founderUserId: founderUserModel.UserId,
                                                        founderConnectionId: inviteDuel.FounderConnectionId,
                                                        founderProfilePicturePath: founderUserModel.ProfilePicturePath,
                                                        founderFullname: founderUserModel.FullName,
                                                        founderLanguage: CurrentUserLanguage,

                                                        subCategoryName: subCategory.SubCategoryName,
                                                        subCategoryPicture: subCategory.SubCategoryPicturePath,

                                                        subCategoryId: inviteDuel.SubCategoryId,
                                                        balanceType: inviteDuel.BalanceType,
                                                        isOpponentOpenSubCategory: subCategory.IsSubCategoryOpen,
                                                        bet: inviteDuel.Bet);

            _eventBus.Publish(@event);

            Logger.LogInformation("Düello daveti gönderildi. {founderUserId} - {opponentUserId}", UserId, inviteDuel.OpponentUserId);

            return Ok(opponentUserModel);
        }

        /// <summary>
        /// Düello davetini kabul et
        /// </summary>
        [HttpPost("AcceptInviteDuel")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AcceptInviteDuel([FromBody] AcceptInviteDuelModel acceptInvite)
        {
            if (acceptInvite == null
                || acceptInvite.SubCategoryId <= 0
                || acceptInvite.Bet < 0
                || acceptInvite.FounderUserId == UserId
                || string.IsNullOrEmpty(acceptInvite.FounderConnectionId)
                || string.IsNullOrEmpty(acceptInvite.FounderUserId)
                || string.IsNullOrEmpty(acceptInvite.OpponentConnectionId))
            {
                return BadRequest();
            }

            BalanceModel balance = await _balanceService.GetBalance(UserId, acceptInvite.BalanceType);
            if (acceptInvite.Bet > balance.Amount)
            {
                return BadRequest(DuelResource.YourBalanceIsInsufficient);
            }

            var eventDuelStart = new DuelStartIntegrationEvent(subCategoryId: acceptInvite.SubCategoryId,
                                                               bet: acceptInvite.Bet,
                                                               balanceType: acceptInvite.BalanceType,
                                                               founderUserId: acceptInvite.FounderUserId,
                                                               founderConnectionId: acceptInvite.FounderConnectionId,
                                                               founderLanguage: acceptInvite.FounderLanguage,
                                                               opponentUserId: UserId,
                                                               opponentConnectionId: acceptInvite.OpponentConnectionId,
                                                               opponentLanguage: CurrentUserLanguage);

            _eventBus.Publish(eventDuelStart);

            return Ok();
        }

        /// <summary>
        /// Rakip ekler
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modu bilgileri</param>
        [HttpPost("AddOpponent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddOpponent([FromBody] StandbyModeModel standbyModeModel)// Oyunucunun karşısına rakip ekler
        {
            if (standbyModeModel.Bet < 0 || standbyModeModel.SubCategoryId <= 0)
            {
                return BadRequest();
            }

            string userId = await _identityService.GetRandomUserId();
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            /* TODO: burada bot eklerken karşısına farklı bir kullanıcı denk gelebilir mi?
             *       yani bot ekle dedik o sırada gerçek kullanıcı denk geldi bot sırada bekler mi?
             *       Eğer sıkıntı olursa AddStandbyMode istek atınca guid oluşturulsun burada rakip eklerken o guide rakip eklensin
            */
            AddWaitingOpponentEvent(userId, standbyModeModel);// Rakip eklendi

            return Ok();
        }

        /// <summary>
        /// Duello rakip bekleme moduna al
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modu bilgileri</param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddStandbyMode([FromBody] StandbyModeModel standbyModeModel)
        {
            if (standbyModeModel.Bet < 0 || standbyModeModel.SubCategoryId <= 0 || string.IsNullOrEmpty(standbyModeModel.ConnectionId))
            {
                return BadRequest();
            }

            BalanceModel balance = await _balanceService.GetBalance(UserId, standbyModeModel.BalanceType);
            if (balance == null || standbyModeModel.Bet > balance.Amount)
            {
                return BadRequest(DuelResource.YourBalanceIsInsufficient);
            }

            AddWaitingOpponentEvent(UserId, standbyModeModel);

            return Ok();
        }

        /// <summary>
        /// Bekleme moduna alma eventi publish eder
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="standbyModeModel">Düello bilgileri</param>
        private void AddWaitingOpponentEvent(string userId, StandbyModeModel standbyModeModel)
        {
            var @event = new WaitingOpponentIntegrationEvent(userId,
                                                             standbyModeModel.ConnectionId,
                                                             standbyModeModel.SubCategoryId,
                                                             standbyModeModel.Bet,
                                                             standbyModeModel.BalanceType,
                                                             CurrentUserLanguage);

            _eventBus.Publish(@event);
        }

        /// <summary>
        /// Rakip bekleme modundan çıkar
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modu bilgileri</param>
        [HttpPost("ExitStandbyMode")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult ExitStandbyMode([FromBody] StandbyModeModel standbyModeModel)
        {
            if (standbyModeModel.Bet < 0 || standbyModeModel.SubCategoryId <= 0 || string.IsNullOrEmpty(standbyModeModel.ConnectionId))
            {
                return BadRequest();
            }

            // TODO: öncesinde kullanıcı gerçekten bekleme modundamı redisden kontrol edilebilir

            var @event = new RemoveWaitingOpponentIntegrationEvent(UserId,
                                                                   standbyModeModel.ConnectionId,
                                                                   standbyModeModel.SubCategoryId,
                                                                   standbyModeModel.Bet,
                                                                   standbyModeModel.BalanceType);

            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Düellodan çık
        /// </summary>
        [HttpPost("{duelId}/DuelEscape")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult DuelEscape([FromRoute] int duelId)
        {
            if (duelId <= 0)
                return BadRequest();

            Logger.LogInformation("Düelodan çıkma isteği geldi. {duelId} {userId}", duelId, UserId);

            if (!_duelRepository.IsDuelFinish(duelId))
            {
                return BadRequest(DuelResource.YouCantLeaveTheFinishedDuel);
            }

            var @event = new DuelEscapeIntegrationEvent(duelId,
                                                        UserId,
                                                        isDuelCancel: false);

            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Düello iptal eder
        /// </summary>
        /// <param name="duelId">Düello id</param>
        [HttpPost("{duelId}/DuelCancel")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult DuelCancel([FromRoute] int duelId)
        {
            if (duelId <= 0)
            {
                duelId = _duelRepository.LastPlayingDuel(UserId);
                if (duelId <= 0)
                    return BadRequest(DuelResource.YouCantLeaveTheFinishedDuel);
            }

            Logger.LogInformation("Düello iptal ediliyor. {duelId} {userId}", duelId, UserId);

            var @event = new DuelEscapeIntegrationEvent(duelId,
                                                        UserId,
                                                        isDuelCancel: true);

            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Düello sonuç ekranını verir
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <returns>Düello sonuç ekranı</returns>
        [HttpGet("{duelId}")]
        [ProducesResponseType(typeof(DuelResultModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DuelResult([FromRoute] int duelId)
        {
            if (duelId <= 0)
                return NotFound();

            DuelResultModel result = _duelRepository.DuelResultByDuelId(duelId, UserId);
            if (result == null)
                return NotFound();

            List<string> userIds = new List<string>
            {
                result.FounderUserId,
                result.OpponentUserId
            };

            IEnumerable<UserModel> users = await _identityService.GetUserInfosAsync(userIds);
            if (users == null || users.Count() < 2)
                return NotFound();

            IEnumerable<UserLevelModel> userLevels = await _subCategoryService.UserLevel(userIds, result.SubCategoryId);

            UserModel founderUser = users.FirstOrDefault(x => x.UserId == result.FounderUserId);
            UserModel opponentUser = users.FirstOrDefault(x => x.UserId == result.OpponentUserId);

            result.FounderFullName = founderUser.FullName;
            result.FounderProfilePicturePath = founderUser.ProfilePicturePath;
            result.FounderUserName = founderUser.UserName;

            if (userLevels != null && userLevels.Any(x => x.UserId == founderUser.UserId))
                result.FounderLevel = userLevels.FirstOrDefault(x => x.UserId == founderUser.UserId).Level;

            result.IsFounder = result.FounderUserId == UserId;

            result.OpponentFullName = opponentUser.FullName;
            result.OpponentProfilePicturePath = opponentUser.ProfilePicturePath;
            result.OpponentUserName = opponentUser.UserName;
            if (userLevels != null && userLevels.Any(x => x.UserId == opponentUser.UserId))
                result.OpponentLevel = userLevels.FirstOrDefault(x => x.UserId == opponentUser.UserId).Level;

            SubCategoryModel subCategoryModel = await _subCategoryService.GetSubCategoryInfo(result.SubCategoryId, CurrentUserLanguage, UserId);
            if (subCategoryModel != null)
            {
                result.SubCategoryName = subCategoryModel.SubCategoryName;
                result.SubCategoryPicturePath = subCategoryModel.SubCategoryPicturePath;
            }

            return Ok(result);
        }

        #endregion Methods
    }
}
