using System;
using ContestPark.Core.Database.Enums;

namespace ContestPark.Core.Database.Interfaces
{
    public interface IEntity
    {
        DateTime? ModifiedDate { get; set; }

        DateTime CreatedDate { get; set; }

        EntityStatus EntityStatus { get; set; }
    }
}