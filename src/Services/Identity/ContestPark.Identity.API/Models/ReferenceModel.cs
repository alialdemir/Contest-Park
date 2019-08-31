using ContestPark.Identity.API.IntegrationEvents.Events;

namespace ContestPark.Identity.API.Models
{
    public class ReferenceModel
    {
        public BalanceTypes BalanceType { get; set; }

        public decimal Amount { get; set; }
    }
}
