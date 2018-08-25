using ContestPark.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContestPark.Core.Model
{
    public abstract class EntityBase : IEntity
    {
        [Column(Order = 9999)]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        [Column(Order = 9998)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}