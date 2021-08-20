using System;
using System.ComponentModel;
using ContestPark.Core.Database.Enums;
using ContestPark.Core.Database.Interfaces;
using Newtonsoft.Json;

namespace ContestPark.Core.Database.Models
{
    public abstract class EntityBase : IEntity
    {
        public DateTime? ModifiedDate { get; set; }

        [ReadOnly(true)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public EntityStatus EntityStatus { get; set; } = EntityStatus.Active;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
