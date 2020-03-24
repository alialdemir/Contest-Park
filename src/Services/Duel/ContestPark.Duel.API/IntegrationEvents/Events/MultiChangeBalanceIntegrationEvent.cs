using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Models;
using ContestPark.EventBus.Events;
using System.Collections.Generic;
using System.Linq;

namespace ContestPark.Duel.API.IntegrationEvents.Events
{
    public class MultiChangeBalanceIntegrationEvent : IntegrationEvent
    {
        public List<ChangeBalanceModel> ChangeBalances { get; } = new List<ChangeBalanceModel>();

        public MultiChangeBalanceIntegrationEvent AddChangeBalance(ChangeBalanceModel changeBalance)
        {
            if (changeBalance != null)
            {
                this.ChangeBalances.Add(changeBalance);
            }

            return this;
        }

        public MultiChangeBalanceIntegrationEvent AddChangeBalance(decimal bet, BalanceTypes balanceType, BalanceHistoryTypes balanceHistoryType, params string[] userIds)
        {
            if (!userIds.Any() || bet <= 0)
                return this;

            foreach (var userId in userIds)
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    this.ChangeBalances.Add(new ChangeBalanceModel
                    {
                        UserId = userId,
                        Amount = bet,
                        BalanceHistoryType = balanceHistoryType,
                        BalanceType = balanceType
                    });
                }
            }

            return this;
        }
    }
}
