using ContestPark.Core.Model;

namespace ContestPark.Infrastructure.Cp.Entities
{
    public class CpEntity : EntityBase
    {
        public string UserId { get; set; }
        public int CpAmount { get; set; }
    }
}