using ContestPark.Duel.API.Models;
using ContestPark.EventBus.Events;
using System.Collections.Generic;

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
    }
}
