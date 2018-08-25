using ContestPark.Core.Model;
using ContestPark.Domain.Cp.Enums;

namespace ContestPark.Infrastructure.Cp.Entities
{
    public class CpInfoEntity : EntityBase
    {
        public int CpInfoId { get; set; }
        public string UserId { get; set; }
        public int CpSpent { get; set; }
        public GoldProcessNames ChipProcessName { get; set; }
    }
}