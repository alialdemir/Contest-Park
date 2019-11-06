using ContestPark.Core.Database.Interfaces;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace ContestPark.Core.Database.Models
{
    public class EntityBase : IEntity
    {
        public DateTime? ModifiedDate { get; set; }

        [ReadOnly(true)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
