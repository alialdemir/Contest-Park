using ContestPark.Core.CosmosDb.Interfaces;
using Newtonsoft.Json;
using System;

namespace ContestPark.Core.CosmosDb.Models
{
    public abstract class DocumentBase : IDocument
    {
        [JsonProperty(PropertyName = "id")]
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}