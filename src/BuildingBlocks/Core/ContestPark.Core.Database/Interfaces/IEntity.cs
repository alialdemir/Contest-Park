using System;

namespace ContestPark.Core.Database.Interfaces
{
    public interface IEntity
    {
        DateTime? ModifiedDate { get; set; }
        DateTime CreatedDate { get; set; }
    }
}
