using ContestPark.Balance.API.Models;
using ContestPark.EventBus.Events;
using System.Collections.Generic;

namespace ContestPark.Balance.API.IntegrationEvents.Events
{
    public class MultiChangeBalanceIntegrationEvent : IntegrationEvent
    {
        public MultiChangeBalanceIntegrationEvent(List<ChangeBalanceModel> changeBalances)
        {
            ChangeBalances = changeBalances;
        }

        public List<ChangeBalanceModel> ChangeBalances { get; }
    }
}
